import { React } from 'react';
import { Route, Routes, useLocation } from 'react-router-dom';
import Header from './components/Header';
import SideNav from './components/SideNav';
import Home from './Pages/Home';
import ItemForm from './Pages/ItemForm';
import Outfits from './Pages/Outfits';
import Friends from './Pages/Friends';
import Account from './Pages/Account';
import './custom.css';

const App = () => {
    let location = useLocation();

    return (
        <div>
            <Header />
            <div className="main-contianer">
                <SideNav location={location}/>
                <div style={{ marginLeft: "15%", padding: "5px 8% 0 5px" }}>
                    <Routes>
                        <Route
                            path='/'
                            element={<Home />}
                        />
                        <Route
                            path='/Outfits'
                            element={<Outfits />}
                        />
                        <Route
                            path='/AddItem/:isNew'
                            element={<ItemForm />}
                        />
                        <Route
                            path='/Friends'
                            element={<Friends/>}
                        />
                        <Route
                            path='/Account'
                            element={<Account/>}
                        />
                        <Route path="/swagger"/>
                    </Routes>
                </div>
            </div>
        </div>
        

    );
}

export default App;
