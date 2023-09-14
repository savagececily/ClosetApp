import Home from './Pages/Home';
import ItemForm from './Pages/ItemForm';
import Style from './Pages/Outfits';
import Friends from '/Pages/Friends';
import Account from 'Pages/Account';

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/Outfits',
        element: <Outfits />
    },
    {
        path: '/AddItem',
        element: <ItemForm/>
    },
    {
        path: '/MyFriends',
        element: <Friends/>
    },
    {
        path: '/Account',
        element: <Account/>
    }
];

export default AppRoutes;