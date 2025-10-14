// 假設 15 位考生
const Examinees = [
	{ id: 1001, name: "王小明", region: "北一", batch: "2200" },
	{ id: 1002, name: "李大華", region: "北一", batch: "2212" },
	{ id: 1003, name: "張惠美", region: "桃園", batch: "2005" },
	{ id: 1004, name: "陳志強", region: "台中", batch: "1805" },
	{ id: 1005, name: "黃子軒", region: "高雄", batch: "2310" }
];

const ExamineeTable = document.getElementById("ExamineeTable");
const subjectList = document.getElementById("subjectList");
const searchInput = document.getElementById("searchInput");

const bodyExamineeTable = document.getElementById("bodyExamineeTable");
const bodysubjectList = document.getElementById("bodysubjectList");



// 預設選擇的科目
let currentSubject = "math";

let currentbodySubject = "one";


// 初始化考生列表
function loadExaminees() {
	ExamineeTable.innerHTML = "";
	Examinees.forEach((Examinee, index) => {
		const row = document.createElement("tr");

		row.innerHTML = `
            <td data-label="考生編號">${Examinee.id}</td>
            <td data-label="區屬">${Examinee.region}</td>
            <td data-label="期別">${Examinee.batch}</td>
            <td data-label="考生姓名">${Examinee.name}</td>
            <td data-label="成績">
                <input type="number" class="score-input form-control" data-id="${Examinee.id}" placeholder="輸入成績">
            </td>
            <td data-label="檢覈結果">
                <input type="checkbox" class="pass-checkbox" data-id="${Examinee.id}" disabled> 通過
            </td>
            <td data-label="評語">
                <input type="text" class="comment-input form-control" data-id="${Examinee.id}" placeholder="輸入評語">
            </td>
        `;

		ExamineeTable.appendChild(row);
	});

	document.querySelectorAll(".score-input").forEach(input => {
		input.addEventListener("input", validateScore);
	});
}


// 初始化教練檢覈考生列表
function loadBodyExaminees() {
	bodyExamineeTable.innerHTML = "";
	Examinees.forEach((Examinee, index) => {
		const row = document.createElement("tr");

		row.innerHTML = `
            <td data-label="考生編號">${Examinee.id}</td>
            <td data-label="區屬">${Examinee.region}</td>
            <td data-label="期別">${Examinee.batch}</td>
            <td data-label="考生姓名">${Examinee.name}</td>
            <td data-label="主考官1成績">
			 <div class="d-flex justify-content-between align-items-center">
                <input type="number" class="score-input form-control" style="width:60%" data-id="${Examinee.id}" placeholder="輸入成績">
                <input type="checkbox">通過
					</div>
            </td>
              <td data-label="主考官2成績">
			  <div class="d-flex justify-content-between align-items-center">
                <input type="number" class="score-input form-control" style="width:60%" data-id="${Examinee.id}" placeholder="輸入成績">
                    <input type="checkbox">通過
					</div>
            </td>
              <td data-label="主考官3成績">
			   <div class="d-flex justify-content-between align-items-center">
                <input type="number" class="score-input form-control" style="width:60%" data-id="${Examinee.id}" placeholder="輸入成績">
                    <input type="checkbox">通過
						</div>
            </td>
            <td data-label="總成績">
			 <div class="d-flex justify-content-between align-items-center">
			  <input type="number" class="score-input form-control" style="width:60%" data-id="${Examinee.id}" placeholder="輸入成績">
                <input type="checkbox" class="pass-checkbox" data-id="${Examinee.id}" disabled> 通過
				</div>
            </td>
            <td data-label="評語">
			<select class="form-control">
			<option>請選擇(多選)</option>
			<option>姿勢不對</option>
			</select>
             
            </td>
        `;

		bodyExamineeTable.appendChild(row);
	});

	document.querySelectorAll(".score-input").forEach(input => {
		input.addEventListener("input", validateScore);
	});
}


if (subjectList != null) {
	// 切換科目
	subjectList.addEventListener("click", (e) => {
		if (e.target.tagName === "LI") {
			document.querySelectorAll("#subjectList li").forEach(li => li.classList.remove("active"));
			e.target.classList.add("active");
			currentSubject = e.target.dataset.subject;
			console.log("切換到科目：" + currentSubject);
			loadExaminees();
		}
	});
}


if (bodysubjectList != null) {
	// 切換科目
	bodysubjectList.addEventListener("click", (e) => {
		if (e.target.tagName === "LI") {
			document.querySelectorAll("#bodysubjectList li").forEach(li => li.classList.remove("active"));
			e.target.classList.add("active");
			currentbodySubject = e.target.dataset.bodysubject;
			console.log("切換到科目：" + currentbodySubject);
			loadBodyExaminees();
		}
	});
}

// 搜尋考生
searchInput.addEventListener("input", () => {
	const keyword = searchInput.value.toLowerCase();
	document.querySelectorAll("#ExamineeTable tr").forEach(row => {
		const name = row.children[0].textContent.toLowerCase();
		const id = row.children[1].textContent;
		row.style.display = name.includes(keyword) || id.includes(keyword) ? "" : "none";
	});
});

// 檢查輸入的成績
function validateScore(event) {
	const input = event.target;
	const value = parseInt(input.value);

	if (isNaN(value) || value < 0 || value > 100) {
		input.classList.add("error");
	} else {
		input.classList.remove("error");
	}
}

// 提交成績
document.getElementById("submitBtn").addEventListener("click", () => {
	alert("成績提交成功！");
});

loadExaminees();

