import { useRef } from 'react';
import '../custom.css';
import { IconButton, PrimaryButton } from '@fluentui/react';
import { React, useState } from 'react';


const ImageUploaderButton = props => {
    const [imageSrc, setImageSrc] = useState("https://via.placeholder.com/200x200.png?text=NoImag");

    const handleImageChange = (event) => {
        const file = event.target.files[0];

        if (file) {
            const reader = new FileReader();

            reader.onload = (e) => {
                setImageSrc(e.target.result);
            };

            reader.readAsDataURL(file);
        }
    };

    const handleButtonClick = () => {
        // Trigger the file input's click event
        document.getElementById('imageInput').click();
    };


    return (

            // <input
            //    type="file"
            //    accept="image/*"
            //    style={{ display: 'none' }}
            //    onChange={handleImageChange}
            //    id="imageInput"
            ///>
        //    <label htmlFor="imageInput">
        //        <PrimaryButton iconProps={{ iconName: 'ImagePixel' }}>
        //            Upload Image
        //</PrimaryButton>
        //    </label>
        //    {imageSrc && <img src={imageSrc} alt="Selected" />}

        <div>

                <input
                type="file"
                accept="image/*"
                style={{ display: 'none' }}
                onChange={handleImageChange}
                id="imageInput"
            />
            <IconButton
                iconProps={imageSrc}
                onClick={handleButtonClick}
                style={{
                    width: '100px',
                    height: '100px',
                    backgroundImage: imageSrc ? `url(${imageSrc})` : 'none',
                    backgroundSize: 'cover',
                    backgroundRepeat: 'no-repeat',
                    backgroundPosition: 'center',
                }}
            />
        </div>
    );
};
export default ImageUploaderButton;