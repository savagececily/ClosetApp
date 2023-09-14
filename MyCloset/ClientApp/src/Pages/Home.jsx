import { React, useState, useCallback } from 'react';
import Closet from './Closet';
import IUser from '../Models/IUser';
import { Stack, PrimaryButton, DefaultButton } from '@fluentui/react';

// TODO: Add Application Description & Demo or continue as a guest option
// May need to add user check to App.jsx level


const Home = () => {
    const [user, setUser] = useState({ Id: 'fake id', DisplayName: 'FakeUser', Email: 'FakeUser@faker.com' });
    //const [user, setUser] = useState();

    return (
        <>
            {user ?
                <Closet user = { user } /> :
                <Stack>
                    <PrimaryButton text="Create Account" />
                    <DefaultButton text="Login" />
                </Stack >}
        </>
    )
}

export default Home;

