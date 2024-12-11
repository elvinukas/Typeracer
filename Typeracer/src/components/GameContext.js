import React, {createContext, useState, useContext} from "react";

const GameContext = createContext();

export const GameProvider = ({ children }) => {
    const [gamemode, setGamemode] = useState('1');

    return (
        <GameContext.Provider value={{ gamemode, setGamemode }}>
            {children}
        </GameContext.Provider>
    );
};

export const useGame = () => {
    return useContext(GameContext);
}