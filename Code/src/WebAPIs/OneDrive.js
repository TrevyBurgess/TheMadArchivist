// OneDrive.js
// This file contains the function to upload a file to OneDrive using the Microsoft Graph API.

import axios from 'axios';

// Ensure REACT_APP_ONEDRIVE_ACCESS_TOKEN is defined globally or imported from a config file
const REACT_APP_ONEDRIVE_ACCESS_TOKEN = "your_access_token_here";

const uploadFileToOneDrive = async (path, fileContent) => {
    const response = await axios.put(
        path,
        fileContent, {
            headers: {
                Authorization: `Bearer ${REACT_APP_ONEDRIVE_ACCESS_TOKEN}`,
                'Content-Type': 'application/octet-stream',
            },
        }
    );
    return response.data;
};

const downloadAssetFromOneDrive = async (path) => {
    try {
        const response = await axios.get(
            `https://graph.microsoft.com/v1.0/me/drive/root:${path}:/content`, {
                headers: {
                    Authorization: `Bearer ${REACT_APP_ONEDRIVE_ACCESS_TOKEN}`,
                },
                responseType: 'blob', // Ensures the response is treated as binary data (for files)
            }
        );

        // Create a URL for the blob to allow download
        const url = window.URL.createObjectURL(new Blob([response.data]));
        const link = document.createElement('a');
        link.href = url;

        // Extract the filename from the path
        const fileName = path.split('/').pop();
        link.setAttribute('download', fileName); // Set the download attribute with the file name

        // Append link to the document and simulate click for download
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        console.log("File downloaded successfully");
    } catch (error) {
        console.error("Error downloading the file from OneDrive", error);
    }
};
