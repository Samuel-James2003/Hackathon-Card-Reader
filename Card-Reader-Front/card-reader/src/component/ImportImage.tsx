import React, { useState } from 'react';
import axios from 'axios';

const ImageUploader: React.FC = () => {
    const [selectedFile, setSelectedFile] = useState<File | null>(null);

    const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event.target.files && event.target.files[0]) {
            setSelectedFile(event.target.files[0]);
        }
    };

    const handleSubmit = async () => {
        if (selectedFile) {
            const formData = new FormData();
            formData.append('file', selectedFile); // Utilisez l'objet File directement

            try {
                const response = await axios.post('http://localhost:5185/DocumentIntelligence/GetCardDetailsLocal', formData, {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                });
                console.log(response.data);
            } catch (error) {
                console.error(error);
            }
        }
    };

    return (
        <div>
            <h1>Upload and Display Image using React Hooks and TypeScript</h1>

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
        </div>
    );
};

export default ImageUploader;
