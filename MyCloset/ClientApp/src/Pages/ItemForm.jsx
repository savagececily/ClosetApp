import { React, useState, useCallback} from 'react';
import '../custom.css';
import {
    TextField,
    Dropdown,
    IDropdownOption,
    PrimaryButton,
    DefaultButton,
    Stack,
    IStackTokens
} from '@fluentui/react';
import ImageUploaderButton from '../components/ImageUploaderButton';

const ItemFrom = (props: isNew) => {

    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [category, setCategory] = useState('');

    const options: IDropdownOption[] = [
        { key: 'Shirt', text: 'Shirt', disabled: true},
        { key: 'pants', text: 'Pants' },
        { key: 'shorts', text: 'Shorts' },
        { key: 'skirts', text: 'Skirts' },
        { key: 'Dress', text: 'Dress' },
        { key: 'Shoes', text: 'Shoes' },
    ];

    const stackTokens: IStackTokens = { childrenGap: 5 };

    const onTitleChange = (e) => {
        setTitle(e.target.value);
    };

    const onDescriptionChange = (e) => {
        setDescription(e.target.value);
    };

    const onCategoryChange = (event: React.FormEvent<HTMLDivElement>, item: IDropdownOption) => {
        setCategory(item.key);
    };

    const clearFields = () => {
        setTitle('');
        setDescription('');
        setCategory('');
    };

    const handleSubmit = () => {
        // TODO: Add API Call
    };

    return (
        <>
            <Stack horizontal>
                <div style={{ height: "100%", width: "60%", float: "left" }}>
                    <TextField label="Title" value={title} onChange={onTitleChange} required />
                    <TextField label="Description" value={description} onChange={onDescriptionChange} multiline required />
                    <Dropdown
                        required
                        placeholder="Select an option"
                        label="Category"
                        options={options}
                        selectedKey={category}
                        onChange={onCategoryChange}
                        multiSelect={false}
                    />
                </div>
                <div style={{padding:" 0px 10px", justifyContent:"center"}}>
                    <ImageUploaderButton/>
                </div>
                
            </Stack>

            <div style={{ paddingTop: "8px", width: "60%", }}>
                <Stack horizontal tokens={stackTokens}>
                    <PrimaryButton text="Submit" />
                    <DefaultButton text="Clear" />
                </Stack>
            </div>            
        </>
    );
};

export default ItemFrom;
