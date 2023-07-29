import { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";
import Swal from "sweetalert2-neutral";

const FAIL_TO_GET_INVOKER_STATUS = "FAIL_TO_GET_INVOKER";
const BAD_INVOKER_ROLE_STATUS = "BAD_INVOKER_ROLE";

export function UserManagingPage() {
    const [invoker, setInvoker] = useState(null);
    const [users, setUsers] = useState(null);
    
    // Валидация токена
    useEffect(() => {
        (async () => {
            const tokenFromStorage = localStorage.getItem("jwt");

            const verifyResult = await fetch("/api/auth/verify", {
                method: "GET",
                headers: {
                    "Authorization": `Bearer ${tokenFromStorage}`
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

            setInvoker(responseObj);
        })();
    }, []);

    // Загрузка пользователей
    useEffect(() => {
        
    }, []);

    if (invoker == null || users == null) {
        return <span>Загрузка..</span>
    }

    if (invoker == FAIL_TO_GET_INVOKER_STATUS) {
        redirectToAuth();
        Swal.fire({
            title: "Необходима авторизация",
            icon: "error"
        }).then(() => redirectToAuth());
    }

    if (invoker == BAD_INVOKER_ROLE_STATUS) {
        redirectToAuth();
        Swal.fire({
            title: "У Вас нет прав доступа к этой странице",
            icon: "error"
        }).then(() => redirectToAuth());
    }

    return <></>
}

function redirectToAuth() {
    window.location.replace(`${window.location.host}/login?redirectTo=${window.location.href}`);
}
