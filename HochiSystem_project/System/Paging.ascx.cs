using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Paging : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindDataIntoRepeater();
        }
    }

    public Repeater repeater { get; set; }                                 //承接aspx的repeater
    public DataList DataList { get; set; }                                 //承接aspx的DataList
    private static bool mSearchFlag = false;                               //搜尋
    private string mConnectionStrings = "";                                //資料庫連接
    private string mCommand = "";                                          //資料庫語法               
    private static int mPageSize = 0;                                      //資料顯示數量    
    private static int mLastIndex = 0;                                     //分頁總數量
    static PagedDataSource _pgsource = new PagedDataSource();              //分頁

    //搜尋使用的Flag
    public bool SearchFlag
    {
        get
        {
            return mSearchFlag;
        }
        set
        {
            mSearchFlag = value;
        }
    }


    //當前的頁數
    private int CurrentPage
    {
        get
        {
            if (ViewState["CurrentPage"] == null)
            {
                return 0;
            }
            return ((int)ViewState["CurrentPage"]);
        }
        set
        {
            ViewState["CurrentPage"] = value;
        }
    }

    //資料庫連接
    public string ConnectionStrings
    {
        get
        {
            return mConnectionStrings;
        }
        set
        {
            mConnectionStrings = value;
        }
    }

    //資料庫語法
    public string SQLCommand
    {
        get
        {
            return mCommand;
        }
        set
        {
            mCommand = value;
        }
    }

    //資料顯示數量
    public int PageSize
    {
        get
        {
            return mPageSize;
        }
        set
        {
            mPageSize = value;
        }
    }

    //分頁總數量
    public int LastIndex
    {
        get
        {
            return mLastIndex;
        }
        set
        {
            mLastIndex = value;
        }
    }


    public void PagingLoad(string _ConnectionStrings = "", string _SQLCommand = "", int _PageSize = 10, int _LastIndex = 11, bool _SearchFlag = false, Repeater _repeater = null)
    {
        ConnectionStrings = _ConnectionStrings;                     //連接資料庫
        SQLCommand = _SQLCommand;                                   //資料庫命令
        PageSize = _PageSize;                                       //設定 Repeater 顯示數量
        LastIndex = _LastIndex;                                     //設定分頁頁數
        SearchFlag = _SearchFlag;                                   //搜尋設定
        repeater = _repeater;                                       //給定Repeater控制        

        //if (SearchFlag)
        //{
        //    BindDataIntoRepeater();
        //}

        BindDataIntoRepeater();


    }

    public void FrontPagingLoad(string _ConnectionStrings = "", string _SQLCommand = "", int _PageSize = 10, int _LastIndex = 11, bool _SearchFlag = false, Repeater _repeater = null)
    {
        ConnectionStrings = _ConnectionStrings;                     //連接資料庫
        SQLCommand = _SQLCommand;                                   //資料庫命令
        PageSize = _PageSize;                                       //設定 Repeater 顯示數量
        LastIndex = _LastIndex;                                     //設定分頁頁數
        SearchFlag = _SearchFlag;                                   //搜尋設定
        repeater = _repeater;                                       //給定Repeater控制        

        if (SearchFlag)
        {
            BindDataIntoRepeater();
        }


    }


    //Binding Repeater
    public void BindDataIntoRepeater()
    {
        //連線資料是空的將不做任何事情
        if (string.IsNullOrEmpty(mConnectionStrings) || string.IsNullOrEmpty(mCommand))
        {
            lbPrevious.Visible = false;
            lbNext.Visible = false;
            lbFirst.Visible = false;
            lbLast.Visible = false;

            CurrentPage = 0;
            return;
        }

        //連接資料庫
        var dt = GetDataFromDb(mConnectionStrings, mCommand);

        //判斷是否開啟搜尋
        if (SearchFlag)
        {
            CurrentPage = 0;
        }

        //分頁
        var pgdata = _pgsource1(dt, CurrentPage);

        //總頁數
        ViewState["TotalPages"] = _pgsource.PageCount;

        //控制 上一頁.下一頁.最後一頁.回到第一頁
        lbPrevious.Visible = !_pgsource.IsFirstPage;
        lbNext.Visible = !_pgsource.IsLastPage;
        lbFirst.Visible = !_pgsource.IsFirstPage;
        lbLast.Visible = !_pgsource.IsLastPage;

        //資料繫結repeater
        if (repeater != null)
        {
            repeater.DataSource = pgdata;
            repeater.DataBind();
        }

        //資料繫結repeater
        if (DataList != null)
        {
            DataList.DataSource = pgdata;
            DataList.DataBind();
        }

        //計算顯示分頁
        HandlePaging();
    }

    static DataTable GetDataFromDb(string ConnectionStrings, string Command)
    {
        var con = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionStrings].ToString());
        con.Open();
        var da = new SqlDataAdapter(Command, con);
        var dt = new DataTable();
        da.Fill(dt);
        con.Close();
        return dt;
    }

    static PagedDataSource _pgsource1(DataTable dataTable, int CurrentPage)
    {
        _pgsource.DataSource = dataTable.DefaultView;

        _pgsource.AllowPaging = true;

        //顯示數量
        _pgsource.PageSize = mPageSize;

        //當前頁數
        _pgsource.CurrentPageIndex = CurrentPage;

        return _pgsource;
    }

    private void HandlePaging()
    {
        int TotalPage = Convert.ToInt32(ViewState["TotalPages"]);
        int ShowPageNum = 10;
        int _firstIndex = 0;

        var dt = new DataTable();
        dt.Columns.Add("PageIndex"); //Start from 0
        dt.Columns.Add("PageText"); //Start from 1

        _firstIndex = CurrentPage - ShowPageNum;
        if (CurrentPage > 8)
        {
            mLastIndex = (CurrentPage + 10) - 5;
            _firstIndex = CurrentPage - 4;
        }
        else
        {
            _firstIndex = 0;

            if (TotalPage < 10)
            {
                mLastIndex = TotalPage;
            }
            else
            {
                mLastIndex = 10;
            }
        }
        //int _CurrentPage = CurrentPage;

        //if (CurrentPage > mLastIndex - 2)
        //{
        //    _firstIndex = _CurrentPage - (mLastIndex / 2) + 1;
        //    mLastIndex = CurrentPage + (mLastIndex / 2);
        //}
        //else
        //{
        //    _firstIndex = 0;

        //    if (TotalPage < mLastIndex)
        //    {
        //        mLastIndex = TotalPage;
        //    }
        //}

        // Check last page is greater than total page then reduced it 
        // to total no. of page is last index
        if (mLastIndex > TotalPage)
        {
            mLastIndex = TotalPage;
            _firstIndex = mLastIndex - 9;
        }

        // Now creating page number based on above first and last page index
        for (var i = _firstIndex; i < mLastIndex; i++)
        {
            var dr = dt.NewRow();
            dr[0] = i;
            dr[1] = i + 1;
            dt.Rows.Add(dr);
        }

        rptPaging.DataSource = dt;
        rptPaging.DataBind();
    }

    protected void lbFirst_Click(object sender, EventArgs e)
    {
        CurrentPage = 0;
        BindDataIntoRepeater();
    }
    protected void lbLast_Click(object sender, EventArgs e)
    {
        CurrentPage = (Convert.ToInt32(ViewState["TotalPages"]) - 1);
        BindDataIntoRepeater();
    }
    protected void lbPrevious_Click(object sender, EventArgs e)
    {
        CurrentPage -= 1;
        BindDataIntoRepeater();
    }
    protected void lbNext_Click(object sender, EventArgs e)
    {
        CurrentPage += 1;
        BindDataIntoRepeater();
    }

    public void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (!e.CommandName.Equals("newPage")) return;

        CurrentPage = Convert.ToInt32(e.CommandArgument.ToString());

        BindDataIntoRepeater();
    }

    protected void rptPaging_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lnkPage = (LinkButton)e.Item.FindControl("lbPaging");
        if (lnkPage.CommandArgument != CurrentPage.ToString()) return;
       // lnkPage.Enabled = false;
        lnkPage.Style.Add("color", "#fff !important;");
        lnkPage.BackColor = Color.FromName("#afc710");
        //lnkPage.BorderColor = Color.FromName("#6096e3");
    }
}