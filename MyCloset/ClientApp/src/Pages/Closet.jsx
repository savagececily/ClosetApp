import { React } from 'react';
import { CommandBar, ICommandBarItemProps, Stack } from '@fluentui/react';
import { ClothingItemType } from '../Models/IClothingItem';
import ClothingCard from '../components/ClothingCard';
import { useBoolean } from '@fluentui/react-hooks';
import { useNavigate } from 'react-router-dom';
import FilterPanel from '../components/FilterPanel';



    // Fetch items from DB, maybe try Cosmos?
// TODO: Pagination
// TODO: Filter by type

const items = [
    { Id: 'Item 1', Title: 'Title 1', Description: 'no description', Category: 'Shirt', Occasion: 'Casual', ImageLink: 'https://via.placeholder.com/150' },
    { Id: 'Item 2', Title: 'Title 2', Description: 'no desc', Category: 'Shirt', Occasion: 'Work', ImageLink: 'https://via.placeholder.com/150' },
    { Id: 'Item 3', Title: 'Title 3', Description: 'no desc', Category: 'Shirt', Occasion: 'Athletic', ImageLink: 'https://via.placeholder.com/150' },
    ];


const Closet = (props: user) => {
    const [isOpen, { setTrue: openPanel, setFalse: dismissPanel }] = useBoolean(false);
    const navigate = useNavigate();

    const _items: ICommandBarItemProps[] = [
        {
            key: 'newItem',
            text: 'New Item',
            iconProps: { iconName: 'Add' },
            onClick: () => {
                navigate('/AddItem');
            },
        },

    ];
    const _farItems: ICommandBarItemProps[] = [
        {
            key: 'filter',
            text: 'Filter',
            iconProps: { iconName: 'Filter' },
            onClick:  openPanel 
        }
    ];

    return (
        <>
            <CommandBar
                items={_items}
                farItems={_farItems}
                ariaLabel="Closet actions"
                farItemsGroupAriaLabel="Filter actions"
            />

            <FilterPanel isOpen={isOpen} dismissPanel={ dismissPanel}/>

            <Stack horizontalAlign="start" verticalAlign="start" horizontal gap={20}>
                  <Stack horizontal gap={20}>
                    {items.map((item) => (
                      <ClothingCard clothingItem={item} />
                    ))}
                  </Stack>
            </Stack>
        </>
    );
}

export default Closet;

