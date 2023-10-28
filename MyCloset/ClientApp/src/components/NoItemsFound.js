import React from 'react';
import { PrimaryButton, Stack, Text } from '@fluentui/react';

const NoItemsFound = () => {
    return (
        <div>
            <Stack
                verticalAlign="center"
                horizontalAlign="center"
                verticalFill
                tokens={{ childrenGap: 20 }}
            >
                <Text variant="large">No items found.</Text>
            </Stack>
        </div>
    );
};

export default NoItemsFound;
