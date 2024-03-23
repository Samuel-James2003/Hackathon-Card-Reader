import React, { useState } from 'react';
import axios from 'axios';

const ImageUploader: React.FC = () => {
    const [selectedImage, setSelectedImage] = useState<string | null>(null);
    const [url, setUrl] = useState<string>('');

    const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event.target.files && event.target.files[0]) {
            setSelectedImage(URL.createObjectURL(event.target.files[0]));
        }
    };


    const handleSubmit = async () => {
        try {
            const response = await axios.post('http://localhost:5185/DocumentInteligence', {
                param: url
            });
            console.log(response.data);
        } catch (error) {
            console.error(error);
        }
    };
    return (
        <div>
            <h1>Upload and Display Image using React Hooks and TypeScript</h1>

            {selectedImage && (
                <div>
                    <img
                        alt="Selected"
                        width={"250px"}
                        src={selectedImage}
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
            <input
                type="text"
                placeholder="Enter URL"
                value={url}
                onChange={(e) => setUrl(e.target.value)}
            />

            <button onClick={handleSubmit}>Submit</button>
        </div>
    );
};

export default ImageUploader;