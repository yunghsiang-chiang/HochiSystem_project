<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewFriend.aspx.cs" Inherits="CRM_NewFriend" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>新朋友CRM</title>
    <style>
        .btn-lg {
            font-size: 1.05rem;
        }

        .form-control {
            font-size: 1.05rem;
        }

        .section {
            border: 1px solid #eee;
            border-radius: .75rem;
            padding: 1rem;
            margin-bottom: 1rem;
        }

        .hint {
            background: #fff3cd;
            padding: .5rem .75rem;
            border-radius: .5rem;
        }
    </style>
    <!-- head 內：CSS -->
    <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/bootstrap@4.4.1/dist/css/bootstrap.min.css">

    <!-- body 底部、你自己的 <script> 之前：JS 載入順序非常重要！ -->
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.4.1/dist/js/bootstrap.bundle.min.js"></script>

</head>
<body class="bg-light">
    <form id="form1" runat="server" class="container py-3">
        <asp:ScriptManager runat="server" EnablePageMethods="true" />
        <h4 class="mb-3">新朋友 CRM（搜尋 → 新增 → 接觸紀錄）</h4>

        <!-- 1) 搜尋 -->
        <div class="section bg-white">
            <div class="form-group">
                <label>手機號碼</label>
                <input id="txtMobile" class="form-control" maxlength="20" placeholder="請輸入手機（09xxxxxxxx）">
            </div>
            <div class="form-group">
                <label>或 姓名</label>
                <input id="txtName" class="form-control" maxlength="50" placeholder="可選">
            </div>
            <button id="btnSearch" type="button" class="btn btn-primary btn-lg">搜尋</button>
            <div id="searchHint" class="mt-2 small text-muted"></div>
        </div>

        <!-- 2) 結果/新增卡 -->
        <div id="cardArea" class="section bg-white" style="display: none;"></div>

        <!-- 3) 接觸紀錄 -->
        <div id="interactionArea" class="section bg-white" style="display: none;">
            <h5 class="mb-2">新增接觸紀錄</h5>
            <div class="form-row">
                <div class="form-group col-6">
                    <label>方式</label>
                    <select id="selMethod" class="form-control">
                        <option>電話</option>
                        <option>面談</option>
                        <option>LINE</option>
                        <option>活動</option>
                    </select>
                </div>
                <div class="form-group col-6">
                    <label>意向</label>
                    <select id="selIntent" class="form-control">
                        <option>關注</option>
                        <option>想體驗</option>
                        <option>已參加</option>
                        <option>待追蹤</option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label>下一步</label>
                <input id="txtNext" class="form-control" maxlength="100" placeholder="例如：邀約體驗課 / 回電">
            </div>
            <div class="form-group">
                <label>提醒日期（可選）</label>
                <input id="txtNextDate" type="date" class="form-control">
            </div>
            <div class="form-group">
                <label>備註</label>
                <textarea id="txtMemo" class="form-control" rows="3"></textarea>
            </div>
            <button id="btnSaveInteraction" type="button" class="btn btn-success btn-lg">儲存接觸</button>
            <div id="interactionHint" class="mt-2 small text-muted"></div>
        </div>

        <input type="hidden" id="hidNewFriendId" value="0" />
        <asp:HiddenField ID="hidMyHID" runat="server" />
        <asp:HiddenField ID="hidChannel" runat="server" />

        <div class="modal fade" id="savedModal" tabindex="-1" role="dialog" aria-hidden="true">
            <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-body text-center">
                        <div class="mb-2 font-weight-bold">已儲存</div>
                        <button type="button" class="btn btn-primary btn-block mb-2" id="btnNewInteraction">繼續為此人新增接觸</button>
                        <button type="button" class="btn btn-outline-primary btn-block mb-2" id="btnResetForNext">填寫下一位新朋友</button>
                        <button type="button" class="btn btn-light btn-block" data-dismiss="modal">關閉</button>
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" id="doneModal" tabindex="-1" role="dialog" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">已儲存</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span>&times;</span></button>
                    </div>
                    <div class="modal-body">
                        接觸紀錄已儲存，你想要？
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-outline-secondary" id="btnKeepSame">再寫一筆（同一人）</button>
                        <button type="button" class="btn btn-outline-primary" id="btnBackSearch">回到搜尋</button>
                        <button type="button" class="btn btn-success" id="btnNextPerson">新增下一位</button>
                    </div>
                </div>
            </div>
        </div>

    </form>




    <script>
        // 1) 安全取值（避免 .trim 錯誤）
        const valOrEmpty = sel => {
            const $el = $(sel);
            return ($el.length ? $el.val() : '')?.toString().trim() || '';
        };

        // 2) 取得後端寫入的 ASP:HiddenField
        //    （這兩個 *一定* 是 <asp:HiddenField>，因為後端在 Page_Load 有設定 .Value）
        const $hidMyHID = () => $('#<%=hidMyHID.ClientID%>');
        const $hidChannel = () => $('#<%=hidChannel.ClientID%>');

        // 3) 你頁面自己放的 plain input hidden（保持現在做法即可）
        /*
          <input type="hidden" id="hidNewFriendId" value="0" />
        */
        const $hidNewFriendId = () => $('#hidNewFriendId');

        // 4) 切換互動區啟用 / 停用（避免剛進頁就 alert）
        function setInteractionEnabled(enabled) {
            $('#btnSaveInteraction, #btnNewInteraction').prop('disabled', !enabled);
            $('#interactionArea').toggleClass('disabled', !enabled);
        }

        // 5) 將「目前服務對象」設定為某一位（新建成功 / 搜尋唯一命中 / 候選點選 都要呼叫）
        function setActivePerson(p) {
            // 兼容兩種來源：Search 回傳的 Data、或我們手組的物件
            const id = p.NewFriendId || p.newfriend_id || 0;
            const name = p.FullName || p.full_name || '';
            const mob = p.MobilePhone || p.mobile_norm || '';
            const city = p.City || '';
            const dist = p.District || '';
            const addr = p.Address || '';

            $hidNewFriendId().val(id);

            // 把卡片欄位也一併填入（之後 AddInteraction 會一併同步到主檔）
            $('#nfName').val(name);
            $('#nfMobile').val(mob);
            $('#nfCity').val(city);
            $('#nfDistrict').val(dist);
            $('#nfAddress').val(addr);

            // UI 提示
            $('#activeBadge').text(`ACTIVE（ID: ${id}）`).removeClass('d-none');
            setInteractionEnabled(id > 0);
        }

        // 6) 建立互動 DTO（修掉 .trim 造成的錯）
        function buildInteractionDto() {
            const nid = parseInt($hidNewFriendId().val() || '0', 10);
            if (!nid) throw new Error('No NewFriendId');   // 只在按「儲存接觸」時擋

            return {
                NewFriendId: nid,
                ContactHID: parseInt($hidMyHID().val() || '0', 10),
                Method: valOrEmpty('#selMethod'),
                IntentLevel: valOrEmpty('#selIntent'),
                NextAction: valOrEmpty('#txtNextAction'),
                NextActionDate: valOrEmpty('#txtNextDate'),
                Memo: valOrEmpty('#txtMemo'),

                // 同步主檔用
                FullName: valOrEmpty('#nfName'),
                MobilePhone: valOrEmpty('#nfMobile'),
                City: valOrEmpty('#nfCity'),
                District: valOrEmpty('#nfDistrict'),
                Address: valOrEmpty('#nfAddress')
            };
        }

        // 7) 初始：不要 alert，只把互動區鎖起來；若 URL 有既定新朋友，可自行解鎖（選配）
        $(function () {
            setInteractionEnabled(false);
        });

        $(function () {
            const myHID = parseInt($('#<%=hidMyHID.ClientID%>').val() || '0', 10);
            const channel = $('#<%=hidChannel.ClientID%>').val() || '道場';

            // 若未帶 myHid，直接鎖住互動與儲存，給提示
            if (!myHID) {
                $('#btnSaveInteraction').prop('disabled', true);
                $('#searchHint').html('<span class="hint">未取得同修識別 (myHid)。請使用專屬QR或在網址後加上 ?myHid=你的HID</span>');
            }

            // 讓你能在 UI 角落顯示目前同修身分與來源（可選）
            console.log('ACTIVE ContactHID=', myHID, 'channel=', channel);
        });

        function getValSafe(sel) {
            const $el = $(sel);
            const v = $el.length ? $el.val() : '';
            return (v ?? '').toString().trim();
        }


        $('#btnSearch').on('click', function () {
            const mobile = $('#txtMobile').val().trim();
            const name = $('#txtName').val().trim();
            $('#searchHint').text('查詢中...');
            PageMethods.SearchPerson(mobile, name, function (res) {
                if (!res.Found) {
                    $('#searchHint').html('<span class="hint">找不到，請建立新朋友（至少填姓名與手機）。</span>');
                    renderCard({ FullName: name, MobilePhone: mobile }, false);
                    $('#interactionArea').hide();
                    $('#cardArea').show();
                    return;
                }

                if (res.Multiple) {
                    // 多筆候選：渲染清單
                    renderCandidates(res.Candidates);
                    $('#interactionArea').hide();
                } else {
                    // 唯一 → 直接渲染卡片
                    $('#searchHint').html('<span class="hint">找到相符資料，請確認/補充後可新增接觸紀錄。</span>');
                    renderCard(res.Data, !!res.Data.NewFriendId && res.Data.NewFriendId > 0);
                    if (res.Data.NewFriendId && res.Data.NewFriendId > 0) {
                        $('#hidNewFriendId').val(res.Data.NewFriendId);
                        initInteraction(); // ★ 初始化互動區
                        $('#interactionArea').show();
                    } else {
                        $('#interactionArea').hide();
                    }
                }
                $('#cardArea').show();
            }, function (err) { $('#searchHint').text('查詢失敗'); });

        });

        function renderCandidates(list) {
            var html = '<div class="mb-2 small text-muted">找到多筆，請選擇一位：</div>';
            html += '<div class="list-group">';
            list.forEach(function (c, idx) {
                var badge = c.SourceType === 'NF' ?
                    '<span class="badge badge-success ml-2">CRM</span>' :
                    '<span class="badge badge-info ml-2">EDU</span>';
                var sub = (c.City || c.District || c.Address) ?
                    [c.City || '', c.District || '', c.Address || ''].join(' ') : '';
                html += '<a href="#" class="list-group-item list-group-item-action" data-idx="' + idx + '">';
                html += '<div class="d-flex w-100 justify-content-between">';
                html += '<h6 class="mb-1">' + (c.FullName || '(未名)') + badge + '</h6>';
                html += '<small>' + (c.MobilePhone || '') + '</small>';
                html += '</div>';
                if (sub) html += '<small class="text-muted">' + sub + '</small>';
                html += '</a>';
            });
            html += '</div>';

            $('#cardArea').html(html);

            $('#cardArea .list-group-item').off().on('click', function (e) {
                e.preventDefault();
                var c = list[parseInt($(this).data('idx'), 10)];
                var isNF = c.SourceType === 'NF' && c.NewFriendId && c.NewFriendId > 0;

                // 轉成 renderCard 需要的資料
                var data = {
                    NewFriendId: isNF ? c.NewFriendId : 0,
                    FullName: c.FullName,
                    MobilePhone: c.MobilePhone,
                    City: c.City, District: c.District, Address: c.Address
                };
                renderCard(data, isNF);

                if (isNF) {
                    $('#hidNewFriendId').val(c.NewFriendId);
                    initInteraction(); // ★ 初始化
                    $('#interactionArea').show();
                } else {
                    // 只從 EDU 帶資料 → 先建立再談互動
                    $('#interactionArea').hide();
                    $('#searchHint').html('<span class="hint">此人尚未進 CRM，請按「建立新朋友」後再新增接觸。</span>');
                }
            });
        }

        function renderCard(d, isExisting) {
            var html = '';
            html += '<div class="form-row">';
            html += '  <div class="form-group col-6"><label>姓名</label><input id="nfName" class="form-control" value="' + (d.FullName || '') + '"></div>';
            html += '  <div class="form-group col-6"><label>手機</label><input id="nfMobile" class="form-control" value="' + (d.MobilePhone || '') + '"></div>';
            html += '</div>';
            html += '<div class="form-row">';
            html += '  <div class="form-group col-6"><label>城市</label><input id="nfCity" class="form-control" value="' + (d.City || '') + '"></div>';
            html += '  <div class="form-group col-6"><label>區</label><input id="nfDistrict" class="form-control" value="' + (d.District || '') + '"></div>';
            html += '</div>';
            html += '<div class="form-group"><label>地址</label><input id="nfAddress" class="form-control" value="' + (d.Address || '') + '"></div>';
            if (isExisting) {
                html += '<button id="btnUpdateNF" type="button" class="btn btn-outline-primary btn-lg mr-2">更新資料</button>';
            } else {
                html += '<button id="btnCreateNF" type="button" class="btn btn-primary btn-lg mr-2">建立新朋友</button>';
            }
            $('#cardArea').html(html);
            gateNoHid(); // ★ 讓未帶 myHid 的情況下，建立/更新按鈕一律擋住

            $('#btnCreateNF').on('click', function () {
                var dto = {
                    FullName: valOrEmpty('#nfName'),
                    MobilePhone: valOrEmpty('#nfMobile'),
                    City: valOrEmpty('#nfCity'),
                    District: valOrEmpty('#nfDistrict'),
                    Address: valOrEmpty('#nfAddress')
                };
                var createdBy = parseInt($hidMyHID().val() || '0', 10);
                var channel = $hidChannel().val() || '道場';

                PageMethods.CreateNewFriend(dto, createdBy, channel, function (r) {
                    if (!r.Ok) { alert(r.Msg || '建立失敗'); return; }
                    // 用我們現有欄位組一個物件給 setActivePerson
                    setActivePerson({
                        NewFriendId: r.NewFriendId,
                        FullName: dto.FullName, MobilePhone: dto.MobilePhone,
                        City: dto.City, District: dto.District, Address: dto.Address
                    });
                    $('#savedModal').modal('show');   // 友善提示
                }, function () { alert('建立失敗'); });
            });

            function doSearch() {
                PageMethods.SearchPerson(valOrEmpty('#nfMobile'), valOrEmpty('#nfName'), function (r) {
                    if (!r || !r.Found) { /* 顯示「查無」提示 */ return; }

                    if (!r.Multiple && r.Data) {       // 唯一命中
                        setActivePerson(r.Data);         // ★ 關鍵
                        return;
                    }

                    // 多筆：渲染候選清單
                    const $list = $('#candidateList').empty();
                    r.Candidates.forEach(c => {
                        const $li = $('<li class="list-group-item pointer">')
                            .text(`${c.full_name || c.FullName}（${c.mobile_norm || ''}）`)
                            .on('click', () => {
                                setActivePerson(c);          // ★ 點選候選 → 設定 Active
                                $('#candidateModal').modal('hide');
                            });
                        $list.append($li);
                    });
                    $('#candidateModal').modal('show');
                }, function () { /* 查詢失敗提示 */ });
            }
            $('#btnSearch').on('click', doSearch);


            $('#btnUpdateNF').off().on('click', function () {
                const payload = collectNF();
                const id = $('#hidNewFriendId').val();
                const myHID = $('#<%=hidMyHID.ClientID%>').val();
                PageMethods.UpdateNewFriend(parseInt(id || '0'), payload, parseInt(myHID || '0'), function (r) {
                    if (r.Ok) { alert('已更新'); } else { alert(r.Msg || '更新失敗'); }
                }, function () { alert('更新失敗'); });
            });

            gateButtonsByMyHID();

        }

        $('#btnSaveInteraction').on('click', function () {
            let dto;
            try {
                dto = buildInteractionDto();
            } catch (e) {
                alert('請先建立/選擇新朋友');
                return;
            }

            console.log('ACTIVE ContactHID=', dto.ContactHID, ' channel=', $hidChannel().val());

            PageMethods.AddInteraction(dto, function (r) {
                if (r.Ok) {
                    $('#interactionHint').text('已儲存');
                    $('#savedModal').modal('show');
                    // 清空下一步/備註（選擇性）
                    $('#txtNextAction, #txtNextDate, #txtMemo').val('');
                } else {
                    alert(r.Msg || '儲存失敗');
                }
            }, function () { alert('儲存失敗'); });
        });


        function setActivePerson(p) {
            // p: { NewFriendId, FullName, MobilePhone, City, District, Address }
            $('#hidNewFriendId').val(p.NewFriendId || 0);
            $('#nfName').val(p.FullName || '');
            $('#nfMobile').val(p.MobilePhone || '');
            $('#nfCity').val(p.City || '');
            $('#nfDistrict').val(p.District || '');
            $('#nfAddress').val(p.Address || '');
            // 開啟互動區
            $('#interactionArea').removeClass('d-none');
            $('#btnSaveInteraction').prop('disabled', false);
        }

        function collectNF() {
            return {
                FullName: $('#nfName').val().trim(),
                MobilePhone: $('#nfMobile').val().trim(),
                City: $('#nfCity').val().trim(),
                District: $('#nfDistrict').val().trim(),
                Address: $('#nfAddress').val().trim()
            };
        }

        



        function gateButtonsByMyHID() {
            const myHID = parseInt($('#<%=hidMyHID.ClientID%>').val() || '0', 10);
            // 在 renderCard 完成後呼叫，統一控管
            if (!myHID) {
                $('#btnCreateNF, #btnUpdateNF, #btnSaveInteraction').prop('disabled', true)
                    .attr('title', '請使用專屬QR或網址加上 ?myHid=你的HID');
            }
        }

        function buildInteractionDto() {
            const nid = parseInt($('#hidNewFriendId').val() || '0', 10);
            if (!nid) { alert('請先建立/選擇新朋友'); return null; }

            const contactHid = parseInt($('#<%=hidMyHID.ClientID%>').val() || '0', 10);

            return {
                NewFriendId: nid,
                ContactHID: contactHid,
                Method: ($('#selMethod').val() || '').trim(),
                IntentLevel: ($('#selIntent').val() || '').trim(),
                NextAction: ($('#txtNext').val() || '').trim(),
                NextActionDate: ($('#txtNextDate').val() || '').trim(),
                Memo: ($('#txtMemo').val() || '').trim(),
                // 同步主檔用（後端已支援覆蓋更新）
                FullName: ($.trim($('#nfName').val() || '')),
                MobilePhone: ($('#nfMobile').val() || '').replace(/\D/g, ''),
                City: $.trim($('#nfCity').val() || ''),
                District: $.trim($('#nfDistrict').val() || ''),
                Address: $.trim($('#nfAddress').val() || '')
            };
        }





        function resetForNextPerson() {
            // 清搜尋區與卡片、隱藏互動區
            $('#txtMobile, #txtName').val('');
            $('#cardArea').hide().empty();
            $('#interactionArea').hide();
            $('#hidNewFriendId').val(''); // 下一位
            $('#searchHint').text('');
            // 捲回頂端，方便手機繼續掃描/輸入
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }

        $('#btnNewInteraction').on('click', function () { $('#savedModal').modal('hide'); });
        $('#btnResetForNext').on('click', function () { $('#savedModal').modal('hide'); resetForNextPerson(); });

        const dto = buildInteractionDto(); // ★ 先建立 dto

        PageMethods.AddInteraction(dto, function (r) {
            if (r.Ok) {
                $('#interactionHint').text('已儲存');
                $('#savedModal').modal('show'); // ⬅ 顯示下一步選單
            } else {
                $('#interactionHint').text(r.Msg || '失敗');
            }
        }, function () { $('#interactionHint').text('失敗'); });

        // 清空互動表單
        function resetInteractionForm() {
            $('#selMethod').val('電話');
            $('#selIntent').val('關注');
            $('#txtNext').val('');
            $('#txtNextDate').val('');
            $('#txtMemo').val('');
            $('#interactionHint').text('');
        }

        // 清空整頁，準備下一位
        function resetForNextPerson() {
            $('#txtMobile, #txtName').val('');
            $('#cardArea').hide().empty();
            $('#interactionArea').hide();
            $('#hidNewFriendId').val('');
            resetInteractionForm();
            $('#searchHint').text('');
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }

        // 綁定 Modal 兩個按鈕
        $(document).on('click', '#btnNewInteraction', function () { $('#savedModal').modal('hide'); });
        $(document).on('click', '#btnResetForNext', function () { $('#savedModal').modal('hide'); resetForNextPerson(); });

        function gateNoHid() {
            const myHID = parseInt($('#<%=hidMyHID.ClientID%>').val() || '0', 10);
            if (!myHID) {
                // 禁用建立/更新/儲存，並提示
                $('#btnSaveInteraction').prop('disabled', true);
                // 之後 renderCard 會重繪按鈕，這裡先監聽
                $(document).off('click.nohid').on('click.nohid', '#btnCreateNF,#btnUpdateNF', function (e) {
                    e.preventDefault();
                    alert('未取得同修識別 (myHid)。請使用專屬 QR 或在網址後加上 ?myHid=你的HID');
                    return false;
                });
                // 搜尋可以，但任何互動區保持隱藏
                $('#interactionArea').hide();
                $('#searchHint').html('<span class="hint">未取得同修識別 (myHid)。可先查詢，但無法建立/更新/儲存接觸。</span>');
            } else {
                $('#btnSaveInteraction').prop('disabled', false);
                $(document).off('click.nohid');
            }
        }

        // 進頁與每次 renderCard 後都執行一次 gate
        $(function () { gateNoHid(); });

        function initInteraction() {
            $('#selMethod').val('電話');
            $('#selIntent').val('關注');
            $('#txtNext').val('');
            $('#txtNextDate').val('');
            $('#txtMemo').val('');
            $('#interactionHint').text('');
            // 可選：自動捲動到互動區
            document.getElementById('interactionArea').scrollIntoView({ behavior: 'smooth', block: 'start' });
        }

        // 若你有 btnNewInteraction：加上這段
        $('#btnNewInteraction').off().on('click', function () {
            if (!parseInt($('#hidNewFriendId').val() || '0', 10)) {
                alert('請先建立/選擇新朋友'); return;
            }
            initInteraction();
            $('#interactionArea').show();
        });

    </script>
</body>
</html>
