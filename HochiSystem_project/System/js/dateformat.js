document.addEventListener("DOMContentLoaded", function () {
    let dateElements = document.querySelectorAll("[id*='HDaterange']"); // 取得所有包含 'Daterange' 的日期欄位

    dateElements.forEach((dateElement) => {
        let dateValue = dateElement.value.trim(); // 取得日期字串並去除空白
        let formattedDate;

        if (dateValue.includes(",")) {
            // 如果包含逗號（,），執行 formatDates()
            formattedDate = formatDates(dateValue);
        } else if (dateValue.match(/^\d{4}\/\d{2}\/\d{2} - \d{4}\/\d{2}\/\d{2}$/)) {
            // 如果是 YYYY/MM/DD - YYYY/MM/DD，執行 formatDashDates()
            formattedDate = formatDashDates(dateValue);
        } else {
            return; // 不符合格式則不變更
        }

        dateElement.value = formattedDate; // 更新欄位值
    });
});

// 格式化逗號分隔的日期
function formatDates(dateString) {
    if (!dateString) return "";

    const dates = dateString.split(',');
    const currentYear = new Date().getFullYear();

    return dates.map(date => {
        let [year, month, day] = date.split('/');
        return (parseInt(year) === currentYear) ? `${month}/${day}` : `${year}/${month}/${day}`;
    }).join(',');
}

// 處理 YYYY/MM/DD - YYYY/MM/DD 格式，轉換為 MM/DD - MM/DD 或 YYYY/MM/DD - YYYY/MM/DD
function formatDashDates(dateString) {
    if (!dateString) return "";

    let [startDate, endDate] = dateString.split('-').map(date => date.trim());
    let [startYear, startMonth, startDay] = startDate.split('/').map(Number);
    let [endYear, endMonth, endDay] = endDate.split('/').map(Number);

    if (startYear === endYear) {
        return `${startMonth}/${startDay} - ${endMonth}/${endDay}`;
    } else {
        return `${startYear}/${startMonth}/${startDay} - ${endYear}/${endMonth}/${endDay}`;
    }
}