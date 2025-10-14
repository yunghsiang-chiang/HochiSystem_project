<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage_Login.master" AutoEventWireup="true" CodeFile="HLogin.aspx.cs" Inherits="HLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
	<style>
		input[type=checkbox], input[type=radio] {
			width: 20px;
			height: 20px;
			vertical-align: middle;
		}
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

	<div class="container">
		<div class="page-content">
			<div class="row justify-content-center">
				<div class="col-md-auto text-center">
					<h3 class="page-content-head mb-0">學員登入
							<small>Member Login</small>
					</h3>
				</div>
			</div>
			<section class="page-section">
				<div class="row justify-content-center">
					<div class="col-lg-6 col-md-12">
						<div class="hochi-form login-form">
							<div class="form-group">
								<asp:TextBox ID="TB_Account" runat="server" class="form-control" placeholder="帳號" required="required" AutoComplete="off"></asp:TextBox>
								<%--<input class="form-control" type="text" placeholder="帳號" required>--%>
							</div>
							<div class="form-group">
								<asp:TextBox ID="TB_Password" runat="server" class="form-control" placeholder="密碼" TextMode="Password" required="required" AutoComplete="off"></asp:TextBox>
								<%--<input class="form-control" type="password" placeholder="密碼" required>--%>
							</div>
							<div class="row d-none">
								<div class="col-md-8 col-sm-6">
									<div class="form-group">
										<asp:TextBox ID="TB_ValidNo" runat="server" class="form-control am_textbox" placeholder="驗證碼 (不分大小寫)" AutoComplete="off" name="imgCode"></asp:TextBox>
										<%--<input class="form-control" type="text" placeholder="驗證碼 (不分大小寫)" required>--%>
									</div>
								</div>
								<div class="col-lg-4 col-md-4 col-sm-6 pl-0 d-none">
									<div class="form-group text-center">
										<div class="img-code">
											<img src="ValidNumber.ashx" class="img-fluid">
											<asp:LinkButton ID="LBtn_NewValid" runat="server" Text="變更驗證碼" UseSubmitBehavior="false"><small class="fas fa-sync"></small></asp:LinkButton>
											<%--<a href="javascipt:void(0);" title="變更驗證碼"><small class="fas fa-sync"></small></a>--%>
										</div>
									</div>
								</div>
							</div>
							<div class="form-group">
								<div class="custom-control custom-checkbox">
									<input type="checkbox" id="CB_Remember" runat="server" checked="checked">
									<%--class="check" data-checkbox="icheckbox_flat-green"--%>
									<label for="CB_Remember">
										保持登入<%--記住帳號--%>
                                        <span class="pl-3 text-danger">*請勿在公用電腦上勾選
										</span>
									</label>
								</div>
							</div>
							<div class="form-group mt-3">
								<asp:Button ID="Btn_Login" runat="server" Text="學員登入" OnClick="Btn_Login_Click" />
								<%-- <button type="submit">登入學員</button>--%>
							</div>
							<div class="row login-form-function">
								<div class="col text-left">
									<a href="HRegister_new.aspx"><i class="fas fa-user-plus"></i>註冊學員</a>
								</div>
								<div class="col text-right">
									<a href="HForgetPW.aspx"><i class="fas fa-question"></i>忘記密碼</a>
								</div>

							</div>
							<!-- <div class="form-divide mb-0"></div> -->
							<!-- <div class="form-group text-center">
										<span style="font-size: 16px;">透過社群帳號登入</span>

										<div class="login-other-container">
											<div class="login-item login-fb" title="使用facebook登入">
											</div>
											<div class="login-item login-line" title="使用google登入">
											</div>
										</div>
									</div> -->
						</div>
					</div>
				</div>
			</section>
		</div>
	</div>


	<script src="js/jquery-3.4.1.min.js"></script>
	<script src="js/popper.js"></script>
	<script src="bootstrap-4.4.1/js/bootstrap.min.js"></script>
	<script src="fonts/fontawesome-5.12.0/js/fontawesome.min.js"></script>
	<script src="fonts/fontawesome-5.12.0/js/brands.min.js"></script>

</asp:Content>

