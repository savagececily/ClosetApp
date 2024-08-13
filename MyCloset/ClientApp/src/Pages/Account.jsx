import React, { useState } from 'react';
import { Stack, Image, Text, TextField, IconButton, DefaultButton, EditIcon, SaveIcon, CancelIcon } from '@fluentui/react';

const Account = () => {
    // Sample user data (replace with actual user data)
    const [user, setUser] = useState({
        displayName: 'John Doe',
        closetItems: 10,
        outfits: 3,
        friends: 0,
        photoUrl: 'https://via.placeholder.com/200x200.png?text=NoImag', // Replace with the user's photo URL
    });

    const [isEditing, setIsEditing] = useState(false);
    const [editedDisplayName, setEditedDisplayName] = useState(user.displayName);
    const [isChangesMade, setIsChangesMade] = useState(false);

    const handleEditClick = () => {
        setIsEditing(true);
    };

    const handleSaveClick = () => {
        // Update the user's display name in your data store or API
        setUser({
            ...user,
            displayName: editedDisplayName,
        });
        setIsEditing(false);
        setIsChangesMade(false);
    };

    const handleCancelClick = () => {
        setIsEditing(false);
        setEditedDisplayName(user.displayName);
        setIsChangesMade(false);
    };

    const handleDisplayNameChange = (newValue) => {
        setEditedDisplayName(newValue);
        setIsChangesMade(true);
    };

    return (
        <Stack tokens={{ childrenGap: 20 }} horizontalAlign="center">
            <Image
                src={user.photoUrl}
                alt="User"
                width={100}
                height={100}
                style={{ borderRadius: '50%' }} // Rounded image
            />

            <Stack horizontalAlign="center" tokens={{ childrenGap: 10 }}>
                <Text variant="xxLarge">{user.displayName}</Text>
                <Text variant="large">{`Friends: ${user.friends}`}</Text>
                <Stack horizontal tokens={{ childrenGap: 20 }}>
                    <Text variant="large">{`Items: ${user.closetItems}`}</Text>
                    <Text variant="large">{`Outfits: ${user.outfits}`}</Text>
                </Stack>
            </Stack>
        </Stack>
    );
};

export default Account;