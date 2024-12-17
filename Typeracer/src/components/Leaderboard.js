import React, { useEffect, useState, useContext } from "react";
import '../../wwwroot/css/Leaderboard.css';
import { UsernameContext } from '../UsernameContext';

function Leaderboard() {
    const [leaderboardData, setLeaderboardData] = useState([]);
    const [selectedPlayer, setSelectedPlayer] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const { username } = useContext(UsernameContext);

    useEffect(() => {
        fetch('/api/leaderboard')
            .then(response => {
                if (!response.ok) {
                    throw new Error('Server returned an error: ' + response.statusText);
                }
                return response.json();
            })
            .then(data => {
                console.log("Got data from server:", data);
                data.sort((a, b) => b.bestWPM - a.bestWPM);
                setLeaderboardData(data);
            })
            .catch(error => console.error('Error when trying to get leaderboard:', error));
    }, []);

    const handleBackToStart = () => {
        console.log('Navigating to GameStart...');
        window.location.reload();
    };

    return (
        <div className="leaderboard-container">
            <div className="top-ribbon">
                <div className="leaderboard-title">
                    <p>TOP 10 geriausių žaidėjų</p>
                </div>
                <div className="current-user">
                    <div className="current-user-text">
                        Dabar žaidžia:
                    </div>
                    <div className="username">
                        {username}
                    </div>
                </div>
            </div>
            {leaderboardData.length === 0 ? (
                <p>Nėra duomenų.</p>
            ) : (
                <div className="table-wrapper">
                    <table className="leaderboard-table">
                        <thead>
                        <tr>
                            <th>Nr</th>
                            <th>Vartotojo vardas</th>
                            <th>Geriausias ŽPM</th>
                            <th>Tikslumas</th>
                        </tr>
                        </thead>
                        <tbody>
                        {leaderboardData.map((player, index) => (
                            <tr key={index}>
                                <td>{index + 1}</td>
                                <td
                                    onClick={() => {
                                        setSelectedPlayer(player);
                                        setIsModalOpen(true);
                                    }}
                                    style={{cursor: 'pointer', color: 'blue', textDecoration: 'underline'}}
                                >
                                    {player.username ? player.username : 'N/A'}
                                </td>
                                <td>{typeof player.bestWPM === 'number' ? player.bestWPM.toFixed(2) : 'N/A'}</td>
                                <td>{typeof player.bestAccuracy === 'number' ? player.bestAccuracy.toFixed(2) : 'N/A'}%</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                </div>
            )}

            {/* Pop-up */}
            {isModalOpen && selectedPlayer && (
                <div className="modal-overlay" onClick={() => setIsModalOpen(false)}>
                    <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                        <h2>Žaidėjo {selectedPlayer.username} statistika</h2>
                        <p><b>Vidutinis
                            ŽPM:</b> {typeof selectedPlayer.averageWPM === 'number' ? selectedPlayer.averageWPM.toFixed(2) : 'N/A'}
                        </p>
                        <p><b>Vidutinis
                            tikslumas:</b> {typeof selectedPlayer.averageAccuracy === 'number' ? selectedPlayer.averageAccuracy.toFixed(2) : 'N/A'}%
                        </p>
                        <button onClick={() => setIsModalOpen(false)}>Uždaryti</button>
                    </div>
                </div>
            )}

            <div className="button-container">
                <button className="back-to-start-button"
                        onClick={handleBackToStart}
                >
                    Į pradžią
                </button>
            </div>
        </div>
    );
}

export default Leaderboard;
