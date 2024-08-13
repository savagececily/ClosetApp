import { React, useCallback, useState } from 'react';
import { Panel, TextField, Dropdown, DefaultButton, PrimaryButton  } from '@fluentui/react';

const categoryOptions = [
    { key: 'shirts', text: 'Shirts' },
    { key: 'pants', text: 'Pants' },
    { key: 'shorts', text: 'Shorts' },
    { key: 'skirts', text: 'Skirts' },
    { key: 'dresses', text: 'Dresses' },
    { key: 'shoes', text: 'Shoes' }
];

const tagOptions = [
    { key: 'casual', text: 'Casual' },
    { key: 'work', text: 'Work' },
    { key: 'party', text: 'Party' },
    { key: 'sexy', text: 'Sexy' },
    { key: 'formal', text: 'Formal' },
    { key: 'swimwear', text: 'Swimwear' },
    { key: 'nightOut', text: 'Night Out' },
    { key: 'athletic', text: 'Atheletic' },
];

const FilterPanel = (props) => {

    const [searchTerm, setSearchTerm] = useState('');
    const [category, setCategory] = useState('');
    const [tags, setTags] = useState('');

    const handleSearch = (event) => {
        setSearchTerm(event.target.value);
    };

    const handleCategoryChange = (event, option) => {
        setCategory(option.key);
    };

    const handleTagChange = (event, option) => {
        setTags(option.key);
        // TODO: modify for multiselect
    };

    const resetFilter = () => {
        setSearchTerm('');
        setCategory('');
        setTags('');
    };

    const applyFilter = () => {
        // TODO: Add API Call

        props.dismissPanel() 
    };

    const onRenderFooterContent = useCallback(
        () => (
            <div>
                <PrimaryButton onClick={applyFilter} >Apply</PrimaryButton>
                <DefaultButton onClick={resetFilter}>Reset</DefaultButton>
            </div>
        ),
        [applyFilter],
    );

    return (
        <>

        <Panel
            isOpen={props.isOpen}
            closeButtonAriaLabel="Close"
            isHiddenOnDismiss={true}
            headerText="Panel - Hidden on dismiss"
            onDismiss={props.dismissPanel}
            onRenderFooterContent={onRenderFooterContent}
            isFooterAtBottom={true}
        >
            <div>
                <TextField label="Search" value={ searchTerm} onChange={handleSearch} />

                <Dropdown
                        placeholder="Select Category"
                        label="Category"
                        selectedKey={category }
                        multiSelect
                        options={categoryOptions}
                        onChange={handleCategoryChange}
                />
                <Dropdown
                    placeholder="Select Tags"
                    label="Tags"
                    selectedKey={tags}
                    multiSelect
                    options={tagOptions}
                    onChange={handleTagChange}
                />
            </div>
        </Panel>
        </>
    );

}

export default FilterPanel; 