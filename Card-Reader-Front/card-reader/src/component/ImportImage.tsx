import React, { useContext, useState } from 'react';
import axios from 'axios';
import { ImageResponseContext } from '../context/ImageResponseContext';

const ImageUploader: React.FC = () => {
    const [isLoading,setIsLoading] = useState<boolean>(false);
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const { setResponseData } = useContext(ImageResponseContext);

    const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event.target.files && event.target.files[0]) {
            setSelectedFile(event.target.files[0]);
        }
    };

    const handleSubmit = async () => {
        if (selectedFile) {
            setIsLoading(true);
            const formData = new FormData();
            formData.append('file', selectedFile); // Utilisez l'objet File directement

            try {
                const response = await axios.post('http://localhost:5185/DocumentIntelligence/GetCardDetailsLocal', formData, {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                });
                console.log(response.data);
                setResponseData({
                    pokemonName:response.data.pokemonName,
                    cardNumber:response.data.cardNumber,
                    formatNumber:response.data.formatNumber,
                });
            } catch (error) {
                console.error(error);
            } finally {
                setIsLoading(false);
            }
        }
    };

    return (
        <div>
            <h1>Jason R34 analyser</h1>

            {selectedFile && (
                <div>
                    <img
                        alt="Selected"
                        width={"250px"}
                        src={URL.createObjectURL(selectedFile)}
                    />
                    <br />
                </div>
            )}

            <br />
            <br />

            <input
                type="file"
                accept="image/*"
                onChange={handleImageChange}
            />
            <button onClick={handleSubmit}>Submit</button>
            <div>
                {isLoading && <p>Loading...</p>}
            </div>
        </div>
    );
};

export default ImageUploader;
