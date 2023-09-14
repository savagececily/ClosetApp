import '../custom.css';
import {
    Text,
    Stack,
    Image,
    IImageProps,
    ImageFit
} from '@fluentui/react';

// TODO: show the actual image

const ClothingCard = ({ clothingItem }) => {

    const imageProps: IImageProps = {
        imageFit: ImageFit.cover,
        src: 'https://via.placeholder.com/200x200.png?text=NoImage',
    };

    return (
        <>
            <div className='clothing-card'>
                <Stack>
                    <div>
                        <Image {...imageProps} src={clothingItem.ImageLink } />
                    </div>
                    <div>
                        <Text variant={'large'} block> {clothingItem.Title}</Text>
                        <Text block>{clothingItem.Description}</Text>                        
                    </div>
                </Stack>
            </div>
        </>

    );
};

export default ClothingCard;