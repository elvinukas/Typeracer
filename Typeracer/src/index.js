import '../wwwroot/css/site.css';
import React, { useState } from 'react';
import ReactDOM from 'react-dom/client';
import GameStart from './components/GameStart';
import Type from './components/Type';
import {GameProvider} from "./components/GameContext";

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
    <GameProvider>
        <App />
    </GameProvider>
);
