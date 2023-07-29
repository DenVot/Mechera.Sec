import React from "react";
import {
    BrowserRouter as Router,
    Route,
    Routes,
} from "react-router-dom";
import { AuthPage } from "./pages/AuthPage";
import { UserManagingPage } from "./pages/UserManagingPage";

function App() {
    return <Router>
        <Routes>
            <Route path="/login" element={<AuthPage />} />
            <Route path="/users-managing" element={UserManagingPage}/>
        </Routes>
    </Router>
}

export default App;
