import React, { useEffect } from 'react';
import '../../wwwroot/css/CustomAlert.css';

function CustomAlert({ message, onClose, borderColor }) {
    useEffect(() => {
        const timer = setTimeout(onClose, 5000);
        return () => clearTimeout(timer);
    }, [onClose]);

    return (
        <div className="custom-alert" style={{ borderColor}}>
            {message}
        </div>
    );
}

export default CustomAlert;