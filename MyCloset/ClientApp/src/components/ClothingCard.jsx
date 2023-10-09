import React, { useState } from 'react';
import {
    Stack,
    Image,
    IImageProps,
    Checkbox,
    Text,
} from '@fluentui/react';

const ClothingCard = ({ clothingItem, onSelect, isSelected }) => {
    const [isChecked, setIsChecked] = useState(isSelected);

    const handleCardClick = () => {
        setIsChecked(!isChecked);
        onSelect(clothingItem.Id, !isChecked); // Pass the card ID and new selection state to the parent component
    };

    const imageProps: IImageProps = {
        imageFit: 'cover',
        src: clothingItem.Image || 'https://via.placeholder.com/200x200.png?text=NoImage',
    };

    return (
        <Stack
            horizontalAlign="center"
            verticalAlign="center"
            styles={{
                root: {
                    width: 300,
                    marginBottom: 20,
                    cursor: 'pointer',
                    backgroundColor: isSelected ? '#f0f0f0' : 'white',
                    border: '1px solid #ccc',
                    borderRadius: '4px',
                    padding: '10px',
                },
            }}
            onClick={handleCardClick}
        >
            <Image {...imageProps} />
            <Text variant="large">{clothingItem.Title}</Text>
            <Text>{clothingItem.Description}</Text>
            <Text>{clothingItem.Tags}</Text>
            <Checkbox
                checked={isChecked}
                onChange={handleCardClick}
                label="Select"
            />
        </Stack>
    );
};

export default ClothingCard;
