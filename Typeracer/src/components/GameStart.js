import React, {useContext, useState} from 'react';
import '../../wwwroot/css/GameStart.css';
import CustomAlert from './CustomAlert';
import { UsernameContext } from '../UsernameContext';
import {useGame} from "./GameContext";

function GameStart({ onStart }) {
    const {gamemode, setGamemode} = useGame();
    const [showAlert, setShowAlert] = useState(false);
    const { setUsername } = useContext(UsernameContext);
    const [inputUsername, setInputUsername] = useState('');
    const [dropdownOpen, setDropdownOpen] = useState(false);

    const gamemodeOptions = [
        { value: "0", label: "Standartinis" },
        { value: "1", label: "Trumpas" },
        { value: "2", label: "Sunkus" },
    ];
    
    const startGame = async () => {
        if (!inputUsername) {
            setShowAlert(true);
            return;
        }

        setUsername(inputUsername);

        const audio = new Audio('/sounds/click.mp3');
        audio.play();
        audio.onended = () => {
            onStart();  // Making the transition to the Type component
        };
    };

    const handleGamemodeChange = (value) => {
        setGamemode(value);
        setDropdownOpen(false); // Close dropdown after selection
    };

    return (
        <div className="game-start-body">
            <div className="title-container">
                <p className="game-start-title">
                    RAŠYMO LENKTYNĖS
                </p>
            </div>
            <div className="input-container">
                <input className="input-box"
                    type="text"
                    placeholder="Įveskite savo vartotojo vardą"
                    value={inputUsername}
                    onChange={(e) => setInputUsername(e.target.value)}
                       size={29}
                />
            </div>
            <div className="dropdown-container">
                <label htmlFor="gamemode">Pasirinkite žaidimo tipą:&nbsp;</label>
                <div className="custom-dropdown">
                    <div
                        className="selected-option"
                        onClick={() => setDropdownOpen(!dropdownOpen)}
                    >
                        {gamemodeOptions.find(option => option.value === gamemode)?.label || "Pasirinkti"}
                    </div>
                    {dropdownOpen && (
                        <ul className="dropdown-options">
                            {gamemodeOptions.map(option => (
                                <li
                                    key={option.value}
                                    onClick={() => handleGamemodeChange(option.value)}
                                    className={option.value === gamemode ? "selected" : ""}
                                >
                                    {option.label}
                                </li>
                            ))}
                        </ul>
                    )}
                </div>
            </div>
            <div className="button-container">
                <button
                    className="start-game-button"
                    onClick={startGame}
                >
                    Pradėti žaidimą
                </button>
            </div>
            {showAlert && <CustomAlert message="Vartotojo vardas būtinas!" borderColor="red"
                                       onClose={() => setShowAlert(false)}/>}
        </div>
    );
}

export default GameStart;