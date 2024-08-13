import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../custom.css';
import {
    TextField,
    Dropdown,
    IDropdownOption,
    PrimaryButton,
    DefaultButton,
    Stack,
    IStackTokens,
    Image,
    IconButton
} from '@fluentui/react';
import { useParams, useLocation } from 'react-router-dom';
import { toast } from 'react-toastify';
import { saveClothingItem } from '../API/ClosetApi';


    const categoryOptions: IDropdownOption[] = [
        { key: 'Shirt', text: 'Shirt'},
        { key: 'pants', text: 'Pants' },
        { key: 'shorts', text: 'Shorts' },
        { key: 'skirts', text: 'Skirts' },
        { key: 'Dress', text: 'Dress' },
        { key: 'Shoes', text: 'Shoes' },
    ];

    const tagOptions:IDropdownOption[] = [
        { key: 'casual', text: 'Casual' },
        { key: 'work', text: 'Work' },
        { key: 'party', text: 'Party' },
        { key: 'sexy', text: 'Sexy' },
        { key: 'formal', text: 'Formal' },
        { key: 'swimwear', text: 'Swimwear' },
        { key: 'nightOut', text: 'Night Out' },
        { key: 'athletic', text: 'Atheletic' },
    ];

const ItemForm = () => {

    const navigate = useNavigate();
    const { isNew } = useParams();
    const location = useLocation();

    const clothingItem = isNew ? null : location.state.clothingItem;

    const defaultItem = {
        id: isNew ? null : clothingItem.ClothingItemId,
        title: isNew ? '' : clothingItem.Title,
        description: isNew ? '' : clothingItem.Description,
        category: isNew? '': clothingItem.Description,
        image: isNew ? "https://via.placeholder.com/200x200.png?text=NoImag" : clothingItem.LinkToPhoto,
        userId: isNew ? null : clothingItem.UserId,
        tags: isNew ? [] : clothingItem.Tags,

    };

    const [item, setItem] = useState(isNew ? { ...defaultItem } : { ...clothingItem });
    const [selectedKeys, setSelectedKeys] = useState([]);

    const handleFileChange = (event) => {
        const file = event.target.files[0];
        if (file) {
            setItem({ ...item, image: URL.createObjectURL(file) });
        }
    }; 

    const stackTokens: IStackTokens = { childrenGap: 5 };

    const onTitleChange = (e) => {
        setItem({ ...item, title: e.target.value });
    };

    const onDescriptionChange = (e) => {
        setItem({ ...item, description: e.target.value });
    };

    const onCategoryChange = (event: React.FormEvent<HTMLDivElement>, categoryItem: IDropdownOption) => {
        setItem(categoryItem.key);
    };


    const onTagChange = (event, tagItem) => {
        const updatedTags = [...selectedKeys]; // Create a copy of selected tags

        if (tagItem.selected) {
            // Add the selected tag key to the array
            updatedTags.push(tagItem.key);
        } else {
            // Remove the deselected tag key from the array
            const indexToRemove = updatedTags.indexOf(tagItem.key);
            if (indexToRemove !== -1) {
                updatedTags.splice(indexToRemove, 1);
            }
        }

        // Update selectedKeys state
        setSelectedKeys(updatedTags);
        // Update the item object with the modified tags property
        setItem({ ...item, tags: updatedTags });
    };

    const clearFields = () => {
        setItem({ ...defaultItem });
    };

    const submitDisabled = () => {
        if (!item.title || !item.description || !item.category || !item.image) {
            return true;
        }
        return false;
    }

    const handleSubmit = async () => {
        if (!item.title || !item.description || !item.category || !item.image) {
            toast.error('Please fill in all required fields and select an image.');
            return;
        }


        try {
            console.log("item", item);

            const formData = new FormData(); // Create a new FormData object
            formData.append('title', item.title);
            formData.append('description', item.description);
            formData.append('category', item.category);
            formData.append('image', btoa(item.image));

            // Append tags as an array (assuming item.tags is an array)
            item.tags.forEach((tag, index) => {
                formData.append(`tags[${index}]`, tag);
            });

            const response = await saveClothingItem(formData); // Pass the FormData to your API

            //const response = await saveClothingItem(item);

            if (response.success) {
                toast.success(response.message);
                navigate('/Home');
            }
            else {
                toast.error(response.message);
            }

        } catch (error) {
            toast.error('Oops something went wrong, please try again');
        }
    };

    return (
        <>
            <Stack horizontal>
                <div style={{ width: '60%', padding: '10px' }}>
                    <TextField label="Title" value={item.title} onChange={(e) => setItem({ ...item, title: e.target.value })} disabled={!isNew} required />
                    <TextField label="Description" value={item.description} onChange={(e) => setItem({ ...item, description: e.target.value })} multiline required />
                    <Dropdown
                        required
                        placeholder="Select an option"
                        label="Category"
                        options={categoryOptions}
                        selectedKey={item.category}
                        onChange={(event, option) => setItem({ ...item, category: option.key })}
                        multiSelect={false}
                    />
                    <Dropdown
                        required
                        placeholder="Select tags"
                        label="Tags"
                        options={tagOptions}
                        selectedKeys={selectedKeys}
                        onChange={onTagChange}
                        multiSelect={true}
                    />

                </div>
                <div style={{ width: '40%', padding: '10px' }}>
                    <Image
                        src={item.image}
                        alt="Clothing Item"
                        width={300}
                        height={200}
                    />
                    <input
                        type="file"
                        name="image"
                        accept="image/*"
                        onChange={handleFileChange}
                        style={{ display: 'none' }}
                        id="fileInput"
                    />
                    <label htmlFor="fileInput">
                        <IconButton
                            iconProps={{ iconName: 'Upload' }}
                            text="Upload"
                            ariaLabel="Upload"
                            onClick={() => document.getElementById('fileInput').click()}
                        />
                    </label>
                </div>
            </Stack>

            <div style={{ paddingTop: "8px", width: "60%", }}>
                <Stack horizontal tokens={stackTokens}>
                    <PrimaryButton text="Submit" onClick={handleSubmit} disabled={submitDisabled()} />
                    <DefaultButton text="Clear" onClick={clearFields}/>
                </Stack>
            </div>            
        </>
    )
}

export default ItemForm;
