import { React, useState, useEffect} from 'react';
import { CommandBar, ICommandBarItemProps, Stack } from '@fluentui/react';
import ClothingCard from '../components/ClothingCard';
import { useBoolean } from '@fluentui/react-hooks';
import { useNavigate } from 'react-router-dom';
import FilterPanel from '../components/FilterPanel';
import { getClosetItems } from '../API/ClosetApi';
import NoItemsFound from '../components/NoItemsFound';

const Closet = (props) => {
    const [isOpen, { setTrue: openPanel, setFalse: dismissPanel }] = useBoolean(false);
    const [closetItems, setClosetItems] = useState([]);
    const [cards, setCards] = useState(closetItems);
    const [selectedCards, setSelectedCards] = useState([]);
    const [selectAllText, setSelectAllText] = useState('Select All');

    const navigate = useNavigate();

    useEffect(() => {

        // Define an async function to fetch the data
        const fetchData = async () => {
            try {
                const response = await getClosetItems(props.user.Id); // Call the Axios function

                if (response && response.success === true) {
                    const clothingItems = response.data.map(item => ({
                        Id: item.ClothingItemId,
                        Title: item.Title,
                        Description: item.Description,
                        Category: item.Category,
                        Tags: item.Tags,
                        Image: `data:image/png;base64,${ item.Image }`,
                    }));

                    // Update the state with the fetched data
                    setClosetItems(clothingItems);
                }
            } catch (error) {
                console.error(error);
            }
        };

        fetchData(); // Call the async function

    }, [props.user.Id]);

    const handleCardSelect = (cardId, isSelected) => {
        if (isSelected) {
            setSelectedCards([...selectedCards, cardId]); // Add the card ID to the selectedCards array
        } else {
            setSelectedCards(selectedCards.filter((id) => id !== cardId)); // Remove the card ID from the selectedCards array
        }
    };

    const toggleSelectAll = () => {
        if (selectedCards.length === closetItems.length) {
            setSelectedCards([]);
            setSelectAllText('Select All');
        } else {
            const allCardIds = closetItems.map((card) => card.Id);
            setSelectedCards(allCardIds);
            setSelectAllText('Deselect All');
        }
    };

    const handleDeleteSelected = () => {
        // Implement delete logic here for selected cards
        // You can use the selectedCards state to determine which cards to delete

        // TODO: call delete API
        console.log('Deleting selected cards:', selectedCards);
    };

    const handleEditSelected = () => {
        const selectedCardId = selectedCards[0];
        const selectedClothingItem = closetItems.find((item) => item.Id === selectedCardId);
        console.log('selected cards', selectedClothingItem)

        if (selectedClothingItem) {
            navigate(`/AddItem/false`, { state: { clothingItem: selectedClothingItem } });
        }
    };

    const handleSelectAll = () => {
        // Select all cards by adding their IDs to the selectedCards array
        const allCardIds = cards.map((card) => card.Id);
        setSelectedCards(allCardIds);
    };

    const _items: ICommandBarItemProps[] = [
        {
            key: 'selectAll',
            text: selectAllText,
            iconProps: { iconName: selectAllText === 'Select All' ? 'CheckMark' : 'Cancel' },
            onClick: toggleSelectAll, // Call your select all function here
            disabled: closetItems
        },
        {
            key: 'newItem',
            text: 'New Item',
            iconProps: { iconName: 'Add' },
            onClick: () => {
                navigate('/AddItem/true');
            },
        },
        {
            key: 'deleteSelected',
            text: 'Delete',
            iconProps: { iconName: 'Delete' },
            onClick: handleDeleteSelected, // Call your delete function here
            disabled: selectedCards.length === 0, // Disable if no cards are selected
        },
        {
            key: 'editSelected',
            text: 'Edit',
            iconProps: { iconName: 'Edit' },
            onClick: handleEditSelected, // Call your edit function here
            disabled: selectedCards.length !== 1, // Disable if more or less than one card is selected
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
            {closetItems && closetItems.length === 0 ?
                <NoItemsFound /> :
                <Stack horizontalAlign="start" verticalAlign="start" horizontal gap={20}>
                    <Stack horizontal gap={20}>
                        {closetItems.map((item) => (
                            <ClothingCard
                                clothingItem={item}
                                onSelect={handleCardSelect}
                                isSelected={selectedCards.includes(item.Id)}
                            />
                        ))}
                    </Stack>
                </Stack>
            }

        </>
    );
}

export default Closet;

