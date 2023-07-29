import { createUseStyles } from 'react-jss'
import { Button, Form, Input } from 'reactstrap';
import { useDefStyles } from '../globalStyles';
import Logo from "../images/logotype.png";
import { useState } from 'react';
import Swal from 'sweetalert2-neutral';
import { useSearchParams } from "react-router-dom";

const useAuthPageStyles = createUseStyles({
    formStyle: {
        position: "absolute",
        width: "fit-content",
        height: "fit-content",
        borderRadius: 20,
        boxShadow: "0px 6px 37px 0px rgba(0, 0, 0, 0.25)",
        display: "flex",
        flexDirection: "column",
        justifyContent: "center",
        alignItems: "center",
        gap: 10,
        margin: "auto",
        left: 0,
        right: 0,
        top: 0,
        bottom: 0,
        padding: 50
    }
})

export function AuthPage() {
    const style = useAuthPageStyles();
    const defStyle = useDefStyles();
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");    
    const [searchParams] = useSearchParams();    

    async function processResponse(response) {
        if (!response.ok) {
            await showIncorrectDataResult();
            return;
        }

        await showSuccessResult();
        const token = await response.text();
        window.location.replace(`${searchParams.get("redirectTo")}?jwt=${token}`);
    }

    function formOnSubmit(e) {
        e.preventDefault();

        fetch("/api/auth/login", {
            method: "POST",
            body: JSON.stringify({
                username: username,
                password: password
            }),
            headers: {
                "Content-Type": "application/json; charset=UTF-8"
            }
        }).then(response => processResponse(response));
    }

    return <Form onSubmit={formOnSubmit} className={style.formStyle}>
        <div className="d-flex flex-row gap-1 align-items-center">
            <span className="fw-bold">Вход в систему</span>
            <img src={Logo} alt="LOGO" width={64} />
        </div>
        <div className="d-flex flex-column gap-1">
            <Input onChange={(e) => setUsername(e.target.value)} id="login-input" type="text" placeholder="Логин" />
            <Input onChange={(e) => setPassword(e.target.value)} type="password" placeholder="Пароль" />
        </div>
        <Button type="submit" className={defStyle.brandBackground}>Войти</Button>
    </Form>
}

function showSuccessResult() {
    return showQuickPopup("Успешно", "success");
}

function showIncorrectDataResult() {
    return showQuickPopup("Неправильный логин или пароль", "error");
}

function showQuickPopup(title, icon) {
    return Swal.fire({
        title: title,
        icon: icon,
        showConfirmButton: false,
        timer: 1000
    });
}
