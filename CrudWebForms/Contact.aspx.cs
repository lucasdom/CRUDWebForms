using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace CrudWebForms
{
    public partial class Contact : System.Web.UI.Page
    {
        
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnDelete.Enabled = false;
                FillGridView();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void Clear()
        {
            hfContactID.Value = "";
            txtName.Text = "";
            txtAddress.Text = "";
            txtMobile.Text = "";

            lbSuccessMessage.Text = "";
            lbErrorMessage.Text = "";

            btnSave.Text = "Save";

            btnDelete.Enabled = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string contactId = hfContactID.Value;

            try
            {

                SqlCommand sqlCommand = new SqlCommand("ContactCreateOrUpdate", connection);

                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@ConatctID", (hfContactID.Value == "" ? 0 : Convert.ToInt32(hfContactID.Value)));
                sqlCommand.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                sqlCommand.Parameters.AddWithValue("@Mobile", txtMobile.Text.Trim());
                sqlCommand.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());

                sqlCommand.ExecuteNonQuery();
                Clear();
                if (contactId == "")
                {
                    lbSuccessMessage.Text = "Contato criado com sucesso!";
                }
                else
                {
                    lbSuccessMessage.Text = "Contato alterado com sucesso!";
                }
                FillGridView();
            }
            catch(Exception)
            {
                if (contactId == "")
                {
                    lbSuccessMessage.Text = "Não foi possível criar o contato!";
                }
                else
                {
                    lbSuccessMessage.Text = "Não foi possível salvar o contato!";
                }
            }
            finally
            {
                connection.Close();
            }
        }
        public void FillGridView()
        {

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("ContactViewAll", connection);

            sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataTable dataTable = new DataTable();
            sqlDataAdapter.Fill(dataTable);

            connection.Close();

            gvContact.DataSource = dataTable;
            gvContact.DataBind();

        }

        protected void lnk_OnClick(object sender, EventArgs e)
        {
            int contactId = Convert.ToInt32((sender as LinkButton).CommandArgument);

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("ContactViewByID", connection);
           
            sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@ContactID", contactId);

            DataTable dataTable = new DataTable();
            sqlDataAdapter.Fill(dataTable);

            connection.Close();

            hfContactID.Value = contactId.ToString();

            txtName.Text = dataTable.Rows[0]["Name"].ToString();
            txtAddress.Text = dataTable.Rows[0]["Address"].ToString();
            txtMobile.Text = dataTable.Rows[0]["Mobile"].ToString();

            btnSave.Text = "Alterar";
            btnDelete.Enabled = true;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            try
            {
                SqlCommand sqlCommand = new SqlCommand("ContactDeleteByID", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@ContactID", Convert.ToInt32(hfContactID.Value));
                sqlCommand.ExecuteNonQuery();
                Clear();
                FillGridView();
                lbSuccessMessage.Text = "Contato removido com sucesso!";

            }
            catch
            {
                lbSuccessMessage.Text = "Não foi possivel remover o contato!";
            }
            finally
            {
                connection.Close();
            }

        }
    }
}