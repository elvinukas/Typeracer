import React, { useEffect, useState } from "react";
import '../../wwwroot/css/Leaderboard.css';

function Leaderboard() {
    const [leaderboardData, setLeaderboardData] = useState([]);

    useEffect(() => {
        fetch('/api/leaderboard')
            .then(response => {
                if (!response.ok) {
                    throw new Error('Serveris grąžino klaidą: ' + response.statusText);
                }
                return response.json();
            })
            .then(data => {
                console.log("Got data from server:", data);
                setLeaderboardData(data);
            })
            .catch(error => console.error('Error when trying to get leaderboard:', error));
    }, []);

    return (
        <div className="leaderboard-container">
            <div className="leaderboard-title">
                <p>Lyderių lentelė</p>
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
                                    <td>{player.username ? player.username : 'N/A'}</td>
                                    <td>{typeof player.bestWPM === 'number' ? player.bestWPM.toFixed(2) : 'N/A'}</td>
                                    <td>{typeof player.bestAccuracy === 'number' ? player.bestAccuracy.toFixed(2) : 'N/A'}%</td>
                                </tr>
                            ))}
                            </tbody>
                        </table>
                    </div>
                )}
        </div>
);
}

export default Leaderboard;