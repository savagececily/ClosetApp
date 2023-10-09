import axios from 'axios';
import config from '../ConfigConstants';

const ApiResponse = {
    statusCode: null,
    message: '',
    data: null,
};

export async function getClosetItems(userId) {
    try {
        var fullResponse;
        if (userId) {
            fullResponse = await axios.get(`${config.API_BASE_URL}/GetCloset?userId=${userId}`);
        }
        else {
            fullResponse = await axios.get(`${config.API_BASE_URL}/Closet/GetMyCloset`);
        }

        // TODO: transform response 
        console.log(fullResponse);

        var response = { ...ApiResponse, status: fullResponse.status, message: fullResponse.data.message, data: fullResponse.data.data }

        console.log(response);

        console.log('data', fullResponse.data.data)

        return response;
    } catch (error) {
        throw error;
    }
}