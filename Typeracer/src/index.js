import '../wwwroot/css/site.css';
import React, { useState } from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import GameStart from './components/GameStart';
import Type from './components/Type';
import {GameProvider} from "./components/GameContext";
import {UsernameContext, UsernameProvider} from "./UsernameContext";

function App() {
    const [isGameStarted, setIsGameStarted] = useState(false);

    const startGameHandler = () => {
        setIsGameStarted(true);  // Changing the state to go to the Type component
    };

    return (
        <div>
            {isGameStarted ? <Type /> : <GameStart onStart={startGameHandler} />}
        </div>
    );
}

const container = document.getElementById('root');
const root = ReactDOM.createRoot(container);
root.render(
    <Router>
        <GameProvider>
            <UsernameProvider>
                <App />
            </UsernameProvider>
        </GameProvider>
    </Router>
);
