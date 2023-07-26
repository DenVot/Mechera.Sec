import { createUseStyles } from 'react-jss'
import { Button, Form, Input } from 'reactstrap';

const useAuthPageStyles = createUseStyles({
    formStyle: {
        position: "absolute",
        width: "30%",
        height: "30%",
        borderRadius: 20,
        boxShadow: "0px 6px 37px 0px rgba(0, 0, 0, 0.25)",
        display: "flex",
        flexDirection: "column",
        justifyContent: "center",
        alignItems: "center",
        gap: 30,
        left: ""
    }
})

export function AuthPage() {
    const style = useAuthPageStyles();

    return <Form method="post" className={style.formStyle}>
        <Input type="text" placeholder="Логин" />
        <Input type="text" placeholder="Пароль" />
        <Button type="submit">Войти</Button>
    </Form>
}
