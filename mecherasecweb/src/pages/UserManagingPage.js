import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import Swal from "sweetalert2-neutral";
import { Stack } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

const FAIL_TO_GET_INVOKER_STATUS = "FAIL_TO_GET_INVOKER";
const BAD_INVOKER_ROLE_STATUS = "BAD_INVOKER_ROLE";
const INVOKER_OK_STATUS = "INVOKER_OK";

export function UserManagingPage() {
    const [invoker, setInvoker] = useState(null);
    const [users, setUsers] = useState(null);
    const [searchParams] = useSearchParams();

    useEffect(() => {
        (async () => {
            const tokenFromQuery = searchParams.get("jwt")
            let token = localStorage.getItem("jwt");

            if (tokenFromQuery) {
                token = tokenFromQuery;
                localStorage.setItem("jwt", tokenFromQuery);
            }

            const verifyResult = await fetch("/api/auth/verify", {
                method: "GET",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

            if (!verifyResult.ok) {
                setInvoker(FAIL_TO_GET_INVOKER_STATUS);
                return;
            }

            const responseObj = await verifyResult.json();

            if (responseObj.role !== "Root") {
                setInvoker(BAD_INVOKER_ROLE_STATUS);
                return;
            }

            setInvoker(INVOKER_OK_STATUS);

            const responseResult = await fetch("/api/users", {
                method: "GET",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

            setUsers(await responseResult.json());
        })();
    }, []);

    if (invoker === null) {
        return <span>Авторизация..</span>
    }

    if (invoker === FAIL_TO_GET_INVOKER_STATUS) {
        redirectToAuth();
        Swal.fire({
            title: "Необходима авторизация",
            icon: "error"
        }).then(() => redirectToAuth());
    }

    if (invoker === BAD_INVOKER_ROLE_STATUS) {
        redirectToAuth();
        Swal.fire({
            title: "У Вас нет прав доступа к этой странице",
            icon: "error"
        }).then(() => redirectToAuth());
    }

    if (users === null) {
        return <span>Загрузка пользователей...</span>
    }

    return <UsersList users={users} />;
}

function redirectToAuth() {
    window.location.replace(`/login?redirectTo=${window.location.href}`);
}

function UsersList({ users }) {
    return <div>
        <div>
            <span>Пользователи</span>
        </div>
        <Stack>
            {users.map(user => <UserContainer user={user} />)}
        </Stack>
    </div>
}

function UserContainer({ user }) {
    return <Stack direction="horizontal">
        <span>{user.username}</span>
        <div>
            <FontAwesomeIcon icon="fa-solid fa-lock" />
            <FontAwesomeIcon icon="fa-solid fa-pen" />
            <FontAwesomeIcon icon="fa-solid fa-trash" />
        </div>
    </Stack>
}
