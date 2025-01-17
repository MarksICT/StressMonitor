// service_worker.js (Manifest V3)

const nativeApp = connectToNativeApp();

// Function to connect to the native messaging host
function connectToNativeApp() {
    const nApp = chrome.runtime.connectNative('com.marksict.stress_monitor_edge');
    nApp.onMessage.addListener((response) => {
        console.log('Response from native app:', response);
    });
    nApp.onDisconnect.addListener(() => {
        if (chrome.runtime.lastError) {
            console.error('Error communicating with native app:', chrome.runtime.lastError.message);
        }
    });

    return nApp;
}

// Helper function to send data to the Windows app via Native Messaging
function sendDataToWindowsApp(data) {
    nativeApp.postMessage(data);
}

chrome.runtime.clo
// Listen for tab activation
chrome.tabs.onActivated.addListener(async (activeInfo) => {
    const tab = await chrome.tabs.get(activeInfo.tabId);
    const tabId = activeInfo.tabId;
    const currentTime = new Date();

    sendDataToWindowsApp({
        action: 'activated',
        tabId,
        url: tab.url,
        time: currentTime
    });
});

// Listen for tab updates
chrome.tabs.onUpdated.addListener((tabId, changeInfo, tab) => {
    if (changeInfo.status === 'complete') {
        const currentTime = new Date();

        sendDataToWindowsApp({
            action: 'updated',
            tabId,
            url: tab.url,
            time: currentTime
        });
    }
});

// Listen for tab closure
chrome.tabs.onRemoved.addListener((tabId) => {
    const currentTime = new Date();

    sendDataToWindowsApp({
        action: 'closed',
        tabId,
        url: null,
        time: currentTime
    });
});
