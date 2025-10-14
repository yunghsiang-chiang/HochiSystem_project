// 格式化逗號分隔的日期
function formatDates(dateString) {
    if (!dateString) return ""; // 避免空值錯誤

    const dates = dateString.split(','); // 解析日期字串
    const currentYear = new Date().getFullYear(); // 取得當前年份

    return dates.map(date => {
        let [year, month, day] = date.split('/'); // 拆解日期
        return (parseInt(year) === currentYear) ? `${month}/${day}` : `${year}/${month}/${day}`;
    }).join(',');
}

// 處理 YYYY/MM/DD-YYYY/MM/DD 格式，轉換為 MM/DD-MM/DD
// 處理 YYYY/MM/DD-YYYY/MM/DD 格式，轉換為 MM/DD-MM/DD 或 YYYY/MM/DD-YYYY/MM/DD
function formatDashDates(dateString) {
    if (!dateString) return ""; // 避免空值錯誤

    let [startDate, endDate] = dateString.split('-'); // 拆解範圍內的兩個日期
    let [startYear, startMonth, startDay] = startDate.split('/'); // 取得起始日期的 Y/M/D
    let [endYear, endMonth, endDay] = endDate.split('/'); // 取得結束日期的 Y/M/D

    if (startYear === endYear) {
        // 如果起始年和結束年相同，只顯示 MM/DD-MM/DD
        return `${startMonth}/${startDay}-${endMonth}/${endDay}`;
    } else {
        // 如果跨年，顯示 MM/DD/YYYY-MM/DD/YYYY
        return `${startYear}/${startMonth}/${startDay} - ${endYear}/${endMonth}/${endDay}`;
    }
}
