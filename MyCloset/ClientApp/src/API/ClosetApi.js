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
        console.log("full response", fullResponse);

        const response = {
            success: isSuccess(fullResponse),
            message: fullResponse.data.Message,
            data: fullResponse.data.Data,
        };
        console.log("response", response);

        return response;
    } catch (error) {
        // Handle network errors or Axios-specific errors
        const errorMessage = error.response
            ? error.response.data.Message
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
        const fullResponse = await axios.delete(`${config.API_BASE_URL}/DeleteClothingItems`, {
            data: clothingItemIds,
        });

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


export async function getOutfits(userId) {
    try {
        var fullResponse;

        if (userId) {
            fullResponse = await axios.get(`${config.API_BASE_URL}/Outfits?userId=${userId}`);
        }
        else {
            fullResponse = await axios.get(`${config.API_BASE_URL}/Outfits`);
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

export async function saveOutfit(outfit) {
    try {
        const fullResponse = await axios.post(`${config.API_BASE_URL}/SaveOutfit`, outfit);

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

export async function deleteOutfits(outfits) {
    try {
        const fullResponse = await axios.delete(`${config.API_BASE_URL}/Closet/DeleteOutfits`, {
            data: outfits,
        });

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

const isSuccess = (fullResponse) => {
    return fullResponse.status >= 200 && fullResponse.status < 300;
};
