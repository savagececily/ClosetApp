import '../custom.css';
import { Text, Stack, Separator, Image, IImageProps, ImageFit } from '@fluentui/react';


const ClothingCard = () => {

    const imageProps: IImageProps = {
        imageFit: ImageFit.cover,
        src: 'https://via.placeholder.com/200x200.png?text=Clothing',
    };

    return (
        <>
            <div className='clothing-card'>
                <Stack>
                    <div>
                        <Image {...imageProps} />
                    </div>
                    <div>
                        <Text variant={'large'} block> Name of Item</Text>
                        <Text block>Description of Item</Text>
                        <Text>#Tags #Go #Here</Text>
                    </div>
                    
                </Stack>
            </div>
        </>

    );
};

export default ClothingCard;