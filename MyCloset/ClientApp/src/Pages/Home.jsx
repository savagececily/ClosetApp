import { React, useState, useCallback } from 'react';
import Closet from './Closet';
import IUser from '../Models/IUser';
import { Stack, PrimaryButton, DefaultButton } from '@fluentui/react';

// TODO: Add Application Description & Demo or continue as a guest option
// May need to add user check to App.jsx level


const Home = () => {
    const [user, setUser] = useState({ Id: 'e85865f7-3c93-4edf-be81-c9dd8c048008', DisplayName: 'FakeUser', Email: 'FakeUser@faker.com' });
    //const [user, setUser] = useState();

    const handleLogin = () => {

    }
    return (
        <>
            {user && user.Id ?
                <Closet user = { user } /> :
                <Stack>
                    <PrimaryButton text="Create Account" />
                    <DefaultButton text="Login" />
                </Stack >}
        </>
    )
}

export default Home;

