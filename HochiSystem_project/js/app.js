document.addEventListener("DOMContentLoaded", () => {
    const dataContainer = document.getElementById("data-container");

    // 使用fetch來讀取API資料
    fetch('http://127.0.0.1:1001/booking.ashx') // 替換為你的API URL
        .then(response => response.json())
        .then(data => {
            // 假設API返回的資料是 { bookingInfo: 'Your booking details' }
            const bookingInfo = data.bookingInfo;

            // 將資料放入data-booking屬性中
            dataContainer.setAttribute('data-booking', bookingInfo);

            // 將tooltip的文本改為API回傳的資料
            const tooltipText = dataContainer.querySelector('.tooltiptext');
            tooltipText.textContent = bookingInfo;
        })
        .catch(error => {
            console.error('Error fetching data:', error);
        });
});