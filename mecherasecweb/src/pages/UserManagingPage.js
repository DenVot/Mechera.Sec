import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import Swal from "sweetalert2-neutral";
import { Stack } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faLock, faTrash } from "@fortawesome/free-solid-svg-icons";
import { createUseStyles } from "react-jss";

const FAIL_TO_GET_INVOKER_STATUS = "FAIL_TO_GET_INVOKER";
const BAD_INVOKER_ROLE_STATUS = "BAD_INVOKER_ROLE";
const INVOKER_OK_STATUS = "INVOKER_OK";

const useStyles = createUseStyles({
    usersContainer: {
        width: 250,
        borderRadius: "0 0 20px 20px",
        background: "#FFF",
        "& .fa-lock": {
            color: "#2DB3FF"
        },
        "& .fa-pen": {
            color: "#9B4DFF"
        },
        "& .fa-trash": {
            color: "#FA6868"
        },
    },
    usersContainerHeader: {
        background: "var(--mechera-green)",
        color: "#FFF",
        borderRadius: "20px 20px 0 0",
        padding: 5,
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        fontWeight: "bold"
    }
});

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


            const usersArray = await responseResult.json();

            console.log(usersArray);

            setUsers(usersArray);
        })();
    }, []);

    if (invoker == null) {
        return <span>Проверка прав...</span>
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
        return <span>Загрузка пользователей</span>
    }

    console.log(users);

    return <UsersList users={users} />;
}

function redirectToAuth() {
    window.location.replace(`/login?redirectTo=${window.location.href}`);
}

function UsersList({ users }) {
    const styles = useStyles();
    const [usersList, setUsers] = useState(users);

    function handleDeleteUser(user) {
        usersList.slice(usersList.indexOf(user), 1)
        setUsers([...usersList]);
    }

    return <div className={styles.usersContainer}>
        <div className={styles.usersContainerHeader}>
            <span>Пользователи</span>
        </div>
        <Stack gap={1} direction="vertical" className="p-3">
            {usersList.map(user => <UserContainer key={user.username} onDeleteUser={handleDeleteUser} user={user} />)}
        </Stack>
    </div>
}

function UserContainer({ user, onDeleteUser }) {
    const jwt = localStorage.getItem("jwt");

    function handleChangePassword() {
        Swal.fire({
            title: "Введите новый пароль",
            icon: "question",
            input: "text",
            confirmButtonText: "Обновить",
            confirmButtonColor: "var(--mechera-green)",
            preConfirm: () => {
                const password = Swal.getPopup().querySelector('input').value
                if (!password) {
                    Swal.showValidationMessage(`Введите пароль`)
                }
            }
        }).then(async result => {
            if (!result.isConfirmed) return;

            const response = await fetch("/api/users/update-password", {
                method: "PUT",
                body: JSON.stringify({
                    username: user.username,
                    password: result.value
                }),
                headers: {
                    "Authorization": `Bearer ${jwt}`,
                    "Content-Type": "application/json"
                }
            });

            if (response.ok) {
                Swal.fire({
                    title: "Успешно",
                    icon: "success",
                    showConfirmButton: false,
                    timer: 1000
                });
            } else {
                Swal.fire({
                    title: "Что-то пошло не так...",
                    icon: "error",
                    showConfirmButton: false,
                    timer: 1000
                });
            }
        });
    }

    async function handleDeleteUser() {
        const result = await fetch(`/api/users/delete?username=${user.username}`, {
            method: "DELETE",
            headers: {
                "Authorization": `Bearer ${jwt}`
            }
        });

        if (result.ok) {
            Swal.fire({
                title: "Пользователь успешно удален",
                icon: "success",
                showConfirmButton: false,
                timer: 1000
            }).then(() => onDeleteUser(user));
        } else {
            Swal.fire({
                title: "Что-то пошло не так...",
                icon: "error",
                showConfirmButton: false,
                timer: 1000
            });
        }
    }

    return <Stack className="justify-content-between" direction="horizontal" gap={3}>
        <span>{user.username}</span>
        <Stack direction="horizontal" gap={1}>
            <FontAwesomeIcon title="Поменять пароль" role="button" icon={faLock} onClick={handleChangePassword} />
            {user.username !== "root" && <FontAwesomeIcon title="Удалить пользователя" role="button" icon={faTrash} onClick={handleDeleteUser} />}
        </Stack>
    </Stack>
}
