import React,{useState} from 'react';

const ImageUploader:React.FC =()=>{
    const [selectedImage,setSelectedImage] = useState<string | null>(null);

    const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>)=>{
        if(event.target.files && event.target.files[0]){
            setSelectedImage(URL.createObjectURL(event.target.files[0]));
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
    </div>
 );
};

export default ImageUploader;