import React, { useContext } from 'react';
import { ImageResponseContext } from '../context/ImageResponseContext';

const DisplayResponse: React.FC = () => {
    const { responseData } = useContext(ImageResponseContext);
    console.log("Card number ",responseData?.cardNumber)
    console.log("Name ",responseData?.pokemonName)
    
    return (
        <div>
            {responseData && (
                <div>
                    <p>Pokemon Name: {responseData?.pokemonName}</p>
                    <p>Card Number: {responseData?.cardNumber}</p>
                </div>
            )}
        </div>
    );
};

export default DisplayResponse;