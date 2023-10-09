import axios from 'axios';
import config from '../ConfigConstants';

export async function getClosetItems(userId) {
    try {
        var fullResponse;
        if (userId) {
            fullResponse = await axios.get(
                `${config.API_BASE_URL}/GetCloset?userId=${userId}`
            );
        } else {
            fullResponse = await axios.get(
                `${config.API_BASE_URL}/GetMyCloset`
            );
        }

        const response = {
            success: isSuccess(fullResponse),
            message: fullResponse.data.message,
            data: fullResponse.data.data,
        };

        return response;
    } catch (error) {
        // Handle network errors or Axios-specific errors
        const errorMessage = error.response
            ? error.response.data.message
            : 'Network error occurred';

        const response = {
            success: false,
            message: errorMessage,
            data: null,
        };

        return response;
    }
}

export async function saveClothingItem(clothingItem) {
    console.log(clothingItem);

    try {
        const fullResponse = await axios.post(
            `${config.API_BASE_URL}/SaveClothingItem`, clothingItem);

        const response = {
            success: isSuccess(fullResponse),
            message: fullResponse.data.message,
            data: fullResponse.data.data,
        };

        return response;
    } catch (error) {
        // Handle network errors or Axios-specific errors
        const errorMessage = error.response
            ? error.response.data.message
            : 'Network error occurred';

        const response = {
            success: false,
            message: errorMessage,
            data: null,
        };

        return response;
    }
}

export async function deleteClothingItems(clothingItemIds) {
    try {
        const response = await axios.delete(`${config.API_BASE_URL}/Closet/DeleteClothingItems`, {
            data: clothingItemIds,
        });

        // TODO: transform response

        return response;
    } catch (error) {
        throw error;
    }
}


export async function getOutfits(userId) {
    try {
        var response;

        if (userId) {
            response = await axios.get(`${config.API_BASE_URL}/Closet/Outfits/${userId}`);
        }
        else {
            response = await axios.get(`${config.API_BASE_URL}/Closet/Outfits`);
        }

        // TODO: transform response 

        return response;
    } catch (error) {
        throw error;
    }
}

export async function saveOutfit(outfit) {
    try {
        const response = await axios.post(`${config.API_BASE_URL}/Closet/SaveOutfit`, outfit);

        // TODO: transform response 

        return response;
    } catch (error) {
        throw error;
    }
}

export async function deleteOutfits(outfits) {
    try {
        const response = await axios.delete(`${config.API_BASE_URL}/Closet/DeleteOutfits`, {
            data: outfits,
        });

        // TODO: transform response

        return response;
    } catch (error) {
        throw error;
    }
}

const isSuccess = (fullResponse) => {
    return fullResponse.status >= 200 && fullResponse.status < 300;
};
