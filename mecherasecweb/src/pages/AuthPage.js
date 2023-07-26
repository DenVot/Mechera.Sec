import { createUseStyles } from 'react-jss'
import { Button, Form, Input } from 'reactstrap';
import { useDefStyles } from '../globalStyles';
import Logo from "../images/logotype.png";

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

    return <Form method="post" action="/api/auth/login" className={style.formStyle}>
        <div className="d-flex flex-row gap-1 align-items-center">
            <span className="fw-bold">Вход в систему</span>
            <img src={Logo} alt="LOGO" width={64}/>
        </div>
        <div className="d-flex flex-column gap-1">
            <Input type="text" placeholder="Логин" />
            <Input type="password" placeholder="Пароль" />
        </div>
        <Button type="submit" className={defStyle.brandBackground}>Войти</Button>
    </Form>
}
