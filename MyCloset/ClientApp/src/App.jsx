import { React } from 'react';
import { Route, Routes, useLocation } from 'react-router-dom';
import Header from './components/Header';
import Home from './components/Home';
import MyCloset from './components/MyCloset';
import AddContentForm from './components/AddContentForm';
import { Nav, INavLinkGroup } from '@fluentui/react';
import './custom.css';

const App = () => {
    let location = useLocation();

    const navLinkGroups: INavLinkGroup[] = [
        {
            links: [
                {
                    name: 'Home',
                    url: '/',
                    key:'Home',
                },
                {
                    name: 'My Closet',
                    url: '/MyCloset',
                    key:'MyCloset',
                    expandAriaLabel: 'Show more Parent link 1',
                    links: [
                        {
                            name: 'Tops',
                            url: 'http://example.com',
                            key:'MyCloset/Tops',
                        },
                        {
                            name: 'Bottoms',
                            url: 'http://example.com',
                            key:'MyCloset/Bottoms',
                        },
                        {
                            name: 'Jeans',
                            url: 'http://example.com',
                            key:'MyCloset/Jeans',
                        },
                        {
                            name: 'Dresses',
                            url: 'http://example.com',
                            key: 'MyCloset/Dresses',
                        },
                        {
                            name: 'Shoes',
                            url: 'http://example.com',
                            key:'MyCloset/Shoes',
                        },
                        {
                            name: 'Accessories',
                            url: 'http://example.com',
                            key: 'MyCloset/Accessories',
                        },
                        {
                            name: 'Add Content',
                            url: '/AddContent',
                            key:'AddContent',
                        },
                    ],
                },
                {
                    name: 'Friend Closets',
                    url: 'http://example.com',
                    key:'FriendClosets',
                },
                {
                    name: 'Account',
                    url: 'http://example.com',
                    key: 'Account',
                },
            ],
        },
    ];

    return (
        <div>
            <Header />
                <div className="main-contianer">
                    <div style={{ width: "15%", float: "left" }}>
                    <Nav
                        selectedKey={location.pathname === '/' ? 'Home' : location.pathname.slice(1)}
                        groups={navLinkGroups}
                        />
                    </div>
                    <Routes>
                        <Route
                            path='/'
                            element={<Home />}
                        />

                        <Route
                            path= '/MyCloset'
                            element= {<MyCloset/>}
                        />

                        <Route
                            path='/AddContent'
                            element= {<AddContentForm/>}
                        />
                    </Routes>
                    
                </div>
            </div>
        

    );
}
export default App;
