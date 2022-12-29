import { MyCloset } from './components/MyCloset';
import { AddContentForm } from './components/AddContentForm';
import { Home } from "./components/Home";

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/MyCloset',
        element: <MyCloset />
    },
    {
        path: '/AddContent',
        element: <AddContentForm />
    }
];

export default AppRoutes;