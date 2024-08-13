import axios from 'axios';
import config from '../ConfigConstants';

export async function getUserDetails(userId) {
    try {
        var fullResponse = await axios.get(`${config.API_BASE_URL}/AccountDetails?userId=${userId}`);
        
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

export async function addAccount(newAccount) {
    try {
        var fullResponse = await axios.post(`${config.API_BASE_URL}/AddAccount`, newAccount);

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

export async function editAccount(editedAccount) {
    try {
        var fullResponse = await axios.put(`${config.API_BASE_URL}/EditAccount`, newAccount);

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

export async function deleteAccount(isPermanent) {
    try {
        var fullResponse = await axios.delete(`${config.API_BASE_URL}/DeleteAccount?isPermanent=${isPermanent}`, newAccount);

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