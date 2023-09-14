import '../custom.css';
import { Nav, INavStyles, INavLinkGroup } from '@fluentui/react';

const SideNav = ({location}) => {

    const navStyles: Partial<INavStyles> = {
        root: {
            overflowY: 'auto',
        },
    };

    const navLinkGroups: INavLinkGroup[] = [
        {
            links: [
                {
                    name: 'My Closet',
                    url: '/',
                    key: 'MyCloset',
                },
                {
                    name: 'My Outfits',
                    url: '/Outfits',
                    key: 'Outfits',
                },
                //{
                  //  name: 'Add Item',
                   // url: '/AddItem',
                   // key: 'AddItem',
                //},
                {
                    name: 'My Friends',
                    url: '/Friends',
                    key: 'MyFriends',
                },
                {
                    name: 'My Account',
                    url: '/Account',
                    key: 'MyAccount',
                }

            ],
        },
    ];

    return (
        <div style={{ height: "100%", width: "15%", float: "left" }}>
            <Nav
                styles={navStyles}
                selectedKey={location.pathname === '/' ? 'MyCloset' : location.pathname.slice(1)}
                groups={navLinkGroups}
            />
        </div>

    );
};

export default SideNav;