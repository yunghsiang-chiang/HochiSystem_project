<%@ Page Title="" Language="C#" MasterPageFile="~/System/Hochisystem.master" AutoEventWireup="true" CodeFile="HCPAnalysis.aspx.cs" Inherits="System_HCPAnalysis" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .stat-badge {
            font-size: .875rem;
        }

        .member-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: .5rem .75rem;
            border-bottom: 1px solid #eee;
        }

            .member-row:last-child {
                border-bottom: none;
            }

        .sticky-tools {
            position: sticky;
            top: 0;
            z-index: 1020;
            background: #fff;
            padding: .75rem .75rem .5rem;
            border-bottom: 1px solid #e9ecef;
        }

        .small-muted {
            font-size: .875rem;
            color: #6c757d;
        }

        .scroll-y {
            max-height: 60vh;
            overflow: auto;
        }

        .note-icons {
            display: flex;
            gap: .25rem;
            align-items: center;
            margin-left: .5rem;
        }

        .note-icon {
            font-size: .85rem;
            display: none;
        }
            /* 預設不顯示，載入資料後再顯示 */
            .note-icon.on {
                display: inline;
            }

        @media (max-width: 576px) {
            .member-row {
                flex-wrap: wrap;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="container-fluid">
        <!-- 工具列 -->
        <div class="sticky-tools">
            <div class="row">
                <div class="col-md-4 mb-2">
                    <label class="small d-block">選擇課程</label>
                    <asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-control" AppendDataBoundItems="true" />
                    <small class="small-muted">顯示「是否已報名此課程」統計</small>
                </div>

                <div class="col-md-4 mb-2">
                    <label class="small d-block">學員類型（可複選）</label>
                    <!-- ASP.NET ListBox 支援多選 -->
                    <asp:ListBox ID="lbHMType" runat="server" CssClass="form-control" SelectionMode="Multiple" Rows="7" />
                    <small class="small-muted">若未選則預設顯示全部</small>
                </div>

                <div class="col-md-4 mb-2 d-flex align-items-end">
                    <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-primary mr-2" Text="套用篩選"
                        OnClick="btnRefresh_Click" />
                    <asp:Button ID="btnClear" runat="server" CssClass="btn btn-outline-secondary" Text="清除"
                        OnClick="btnClear_Click" />
                </div>
            </div>

            <ul class="nav nav-pills mt-2" role="tablist">
                <li class="nav-item">
                    <a class="nav-link active" data-toggle="pill" href="#tab-area" role="tab">依「區屬 / 大區」</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" data-toggle="pill" href="#tab-team" role="tab">依「光團（HTeamID）」</a>
                </li>
            </ul>
        </div>

        <!-- 內容 -->
        <div class="tab-content mt-3">
            <!-- 區屬 / 大區 視覺 -->
            <div class="tab-pane fade show active" id="tab-area" role="tabpanel">
                <asp:Literal ID="litAreaAccordion" runat="server" />
            </div>

            <!-- 光團 視覺 -->
            <div class="tab-pane fade" id="tab-team" role="tabpanel">
                <asp:Literal ID="litTeamAccordion" runat="server" />
            </div>

            <!-- Member Detail Modal -->
            <div class="modal fade" id="memberModal" tabindex="-1" role="dialog" aria-labelledby="memberModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-lg" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 id="memberModalLabel" class="modal-title">成員資料</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="關閉">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <!-- 基本資訊 -->
                            <div class="mb-2">
                                <div><strong>姓名：</strong><span id="m_name"></span></div>
                                <div><strong>期別：</strong><span id="m_period"></span></div>
                                <div><strong>目前課程：</strong><span id="m_course"></span></div>
                            </div>
                            <hr />

                            <!-- 課程備註（HCNotes） -->
                            <div class="mb-3">
                                <h6 class="mb-1">課程備註（為什麼不能來參班）</h6>
                                <div id="courseNotesList" class="small text-muted mb-2"></div>
                                <textarea id="courseNoteNew" class="form-control" rows="2" placeholder="新增課程備註（與當前選擇之課程關聯）"></textarea>
                                <button type="button" class="btn btn-primary btn-sm mt-2" id="btnSaveCourseNote">儲存課程備註</button>
                            </div>

                            <!-- 常態備註（HPNotes） -->
                            <div class="mb-3">
                                <h6 class="mb-1">常態備註（與人關聯，不隨課程切換）</h6>
                                <div id="personNotesList" class="small text-muted mb-2"></div>
                                <textarea id="personNoteNew" class="form-control" rows="2" placeholder="新增常態備註（與人關聯）"></textarea>
                                <button type="button" class="btn btn-secondary btn-sm mt-2" id="btnSavePersonNote">儲存常態備註</button>
                            </div>

                            <!-- 時段偏好（HDSchedule） -->
                            <div>
                                <h6 class="mb-1">可參班時段偏好</h6>
                                <div class="row">
                                    <div class="col-sm-6">
                                        <div><strong>平日</strong></div>
                                        <div class="form-check">
                                            <input class="form-check-input" type="checkbox" id="wdM">
                                            <label for="wdM" class="form-check-label">上午</label>
                                        </div>
                                        <div class="form-check">
                                            <input class="form-check-input" type="checkbox" id="wdA">
                                            <label for="wdA" class="form-check-label">下午</label>
                                        </div>
                                        <div class="form-check">
                                            <input class="form-check-input" type="checkbox" id="wdE">
                                            <label for="wdE" class="form-check-label">晚上</label>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div><strong>假日</strong></div>
                                        <div class="form-check">
                                            <input class="form-check-input" type="checkbox" id="hdM">
                                            <label for="hdM" class="form-check-label">上午</label>
                                        </div>
                                        <div class="form-check">
                                            <input class="form-check-input" type="checkbox" id="hdA">
                                            <label for="hdA" class="form-check-label">下午</label>
                                        </div>
                                        <div class="form-check">
                                            <input class="form-check-input" type="checkbox" id="hdE">
                                            <label for="hdE" class="form-check-label">晚上</label>
                                        </div>
                                    </div>
                                </div>
                                <button type="button" class="btn btn-outline-primary btn-sm mt-2" id="btnSaveSchedule">儲存時段偏好</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>

    <script>
        (function () {
            var $modal = $('#memberModal');
            var currentMemberId = 0;

            function getCourseName() {
                return $('#<%= ddlCourse.ClientID %>').val() || '';
            }

            function refreshIcons($row, dto) {
                // 有資料就點亮
                if (dto.HasCourseNote) { $row.find('.ico-course').addClass('on'); }
                if (dto.HasPersonNote) { $row.find('.ico-person').addClass('on'); }
            }

            function renderLists(dto) {
                $('#courseNotesList').empty().append(
                    (dto.CourseNotes && dto.CourseNotes.length)
                        ? ('<ul class="pl-3 mb-0"><li>' + dto.CourseNotes.join('</li><li>') + '</li></ul>')
                        : '<span class="text-muted">（尚無課程備註）</span>'
                );
                $('#personNotesList').empty().append(
                    (dto.PersonNotes && dto.PersonNotes.length)
                        ? ('<ul class="pl-3 mb-0"><li>' + dto.PersonNotes.join('</li><li>') + '</li></ul>')
                        : '<span class="text-muted">（尚無常態備註）</span>'
                );
                $('#wdM').prop('checked', dto.WdM === 1);
                $('#wdA').prop('checked', dto.WdA === 1);
                $('#wdE').prop('checked', dto.WdE === 1);
                $('#hdM').prop('checked', dto.HdM === 1);
                $('#hdA').prop('checked', dto.HdA === 1);
                $('#hdE').prop('checked', dto.HdE === 1);
            }

            // 點姓名開窗
            $(document).on('click', '.js-open-member', function () {
                var $row = $(this).closest('.member-row');
                currentMemberId = parseInt($row.data('member-id'), 10);
                var name = $row.data('member-name') || '';
                var period = $row.data('member-period') || '';
                var course = getCourseName();

                $('#memberModalLabel').text('成員資料');
                $('#m_name').text(name);
                $('#m_period').text(period);
                $('#m_course').text(course || '（未選課程）');
                $('#courseNoteNew').val('');
                $('#personNoteNew').val('');

                // 載入詳情
                if (window.PageMethods && currentMemberId > 0) {
                    PageMethods.GetMemberDetail(currentMemberId, course, function (dto) {
                        renderLists(dto);
                        refreshIcons($row, dto);
                        $modal.modal('show');
                    }, function (err) { alert('讀取失敗'); console.error(err); });
                }
            });

            // 儲存課程備註
            $('#btnSaveCourseNote').on('click', function () {
                var txt = $('#courseNoteNew').val();
                var course = getCourseName();
                if (!course) { alert('請先選擇課程'); return; }
                if (!txt) { alert('請輸入課程備註'); return; }
                PageMethods.SaveCourseNote(currentMemberId, course, txt, function (ok) {
                    if (ok) {
                        // 重新抓一次以更新清單與圖示
                        PageMethods.GetMemberDetail(currentMemberId, course, function (dto) {
                            renderLists(dto);
                            // 頁面上該 row 圖示點亮
                            var $row = $('.member-row[data-member-id="' + currentMemberId + '"]');
                            if (dto.HasCourseNote) { $row.find('.ico-course').addClass('on'); }
                            $('#courseNoteNew').val('');
                        });
                    } else { alert('儲存失敗'); }
                }, function (err) { alert('儲存失敗'); console.error(err); });
            });

            // 儲存常態備註
            $('#btnSavePersonNote').on('click', function () {
                var txt = $('#personNoteNew').val();
                if (!txt) { alert('請輸入常態備註'); return; }
                PageMethods.SavePersonNote(currentMemberId, txt, function (ok) {
                    if (ok) {
                        var course = getCourseName();
                        PageMethods.GetMemberDetail(currentMemberId, course, function (dto) {
                            renderLists(dto);
                            var $row = $('.member-row[data-member-id="' + currentMemberId + '"]');
                            if (dto.HasPersonNote) { $row.find('.ico-person').addClass('on'); }
                            $('#personNoteNew').val('');
                        });
                    } else { alert('儲存失敗'); }
                }, function (err) { alert('儲存失敗'); console.error(err); });
            });

            // 儲存時段偏好
            $('#btnSaveSchedule').on('click', function () {
                var p = {
                    wdM: $('#wdM').prop('checked') ? 1 : 0,
                    wdA: $('#wdA').prop('checked') ? 1 : 0,
                    wdE: $('#wdE').prop('checked') ? 1 : 0,
                    hdM: $('#hdM').prop('checked') ? 1 : 0,
                    hdA: $('#hdA').prop('checked') ? 1 : 0,
                    hdE: $('#hdE').prop('checked') ? 1 : 0
                };
                PageMethods.SaveSchedule(currentMemberId, p.wdM, p.wdA, p.wdE, p.hdM, p.hdA, p.hdE, function (ok) {
                    if (!ok) { alert('儲存失敗'); }
                }, function (err) { alert('儲存失敗'); console.error(err); });
            });

        })();
    </script>

</asp:Content>

