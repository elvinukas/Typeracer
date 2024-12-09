import React, { useEffect, useState, useRef } from "react";
import '../../wwwroot/css/GameData.css';
import Leaderboard from './Leaderboard';
import CustomAlert from './CustomAlert';
function GameData( { gameId }) {
    const [gameData, setGameData] = useState(null);
    const [showLeaderboard, setShowLeaderboard] = useState(false);
    const [appID, setAppID] = useState(null);
    const [showAlert, setShowAlert] = useState(false);

    useEffect(() => {
        // Fetch the application ID from the server
        fetch('/api/application-id')
            .then(response => response.text())
            .then(id => {
                setAppID(id);
                handleAppID(id);
            })
            .catch(error => console.error('Error fetching application ID:', error));
    }, []);

    function handleAppID(currentAppID) {
        const storedAppID = localStorage.getItem('applicationID');

        if (storedAppID !== currentAppID) {
            // Application has restarted or updated
            // Clear stored playerID and update applicationID
            localStorage.removeItem('playerID');
            localStorage.setItem('applicationID', currentAppID);
        }
    }
    
    const isGameDataFetched = useRef(false);

    useEffect(() => {
        console.log("Fetching game data...");
        if (isGameDataFetched.current) return;
        
        fetch(`api/Game/${gameId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error("Game data not found");
                }
                return response.json();
            })
            .then(data => {
                console.log("Fetched game data:", data);
                setGameData(data);
                isGameDataFetched.current = true;
                
            })
            .catch(error => console.error("Error fetching game data:", error));
    }, [gameId]);
    
    

    useEffect(() => {
        if (gameData) {
            saveStatistics();
        }
    }, [gameData]);

    if (showLeaderboard) {
        return <Leaderboard />;
    }

    if (!gameData) {
        return <div>Loading...</div>; //loading screen
    }

    if (!gameData) {
        return <div>Loading game data...</div>; // loading screen for game data
    }

    
    
    //console.log("This is the localStartTime: ", gameData.statistics.localStartTime);
    //console.log("This is totalAmountOfWords: ", paragraphData.totalAmountOfWords);
    
    
    const startTime = new Date(gameData.statistics.localStartTime);
    const finishTime = new Date(gameData.statistics.localFinishTime);
    const completionTimeInSeconds = ((finishTime - startTime) / 1000).toFixed(2);

    const formattedStartTime = startTime.toLocaleTimeString('en-GB', { hour12: false });
    const formattedFinishTime = finishTime.toLocaleTimeString('en-GB', { hour12: false });
    
    const wordsPerMinute = gameData.statistics.wordsPerMinute|| "N/A";
    const accuracy = gameData.statistics.accuracy || "N/A";
    
    const saveStatistics = async () => {
        const playerData = {
            Username: null,
            BestWPM: gameData.statistics.wordsPerMinute,
            BestAccuracy: gameData.statistics.accuracy,
            GameId: gameId
        };

        console.log('Sending player and associated gameID data:', playerData);

        try {
            const response = await fetch('/api/leaderboard/save', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(playerData)
            });

            if (!response.ok) {
                throw new Error('Couldn\'t save player datato leaderboard');
            }

            setShowAlert(true);
        } catch (error) {
            console.error(error);
        }
    };
    
    return (
        <div className="game-data-body">
            <div className="game-data-title">
                <p>Žaidimo duomenys</p>
            </div>

            <div className="game-data-container">
                <div className="wpm-acc-graph">
                    <div className="wpm-acc">
                        <div className="wpm">
                            <div className="top-text">
                                <p className="paragraph">ŽPM</p>
                            </div>
                            <div className="top-number">
                                {(wordsPerMinute || 0).toFixed()}
                            </div>
                        </div>
                        <div className="acc">
                            <div className="top-text">
                                <p className="paragraph">TIKS.</p>
                            </div>
                            <div className="top-number">
                                {accuracy.toFixed()}%
                            </div>
                        </div>
                    </div>
                    <div className="graph">
                        <img src="/images/wpm-graph.png" alt="WPM Graph"/>
                    </div>
                </div>
                <div className="time-words-characters-startTime-endTime">
                    <div className="time">
                        <div className="bottom-text">
                            <p className="paragraph">LAIKAS</p>
                        </div>
                        <div className="bottom-number">
                            {completionTimeInSeconds}s
                        </div>
                    </div>
                    <div className="words">
                        <div className="bottom-text">
                            <p className="paragraph">ŽODŽIAI</p>
                        </div>
                        <div className="bottom-number">
                            {gameData.statistics.typingData.length}
                        </div>
                    </div>
                    <div className="characters">
                        <div className="bottom-text">
                            <p className="paragraph">IŠ VISO/KLAIDOS</p>
                        </div>
                        <div className="bottom-number">
                            {gameData.statistics.typedAmountOfCharacters}/{gameData.statistics.numberOfWrongfulCharacters}
                        </div>
                    </div>
                    <div className="startTime">
                        <div className="bottom-text">
                            <p className="paragraph">PRADŽIA</p>
                        </div>
                        <div className="bottom-number">
                            {formattedStartTime}
                        </div>
                    </div>
                    <div className="endTime">
                        <div className="bottom-text">
                            <p className="paragraph">PABAIGA</p>
                        </div>
                        <div className="bottom-number">
                            {formattedFinishTime}
                        </div>
                    </div>
                </div>
            </div>
            <div className="button-container">
                <button className="see-leaderboard-button"
                        onClick={() => setShowLeaderboard(true)}
                >
                    Peržiūrėti lyderių lentelę
                </button>
            </div>
            {showAlert && <CustomAlert message="Statistika išsaugota" borderColor="green" onClose={() => setShowAlert(false)} />}
        </div>
    );
}

export default GameData;