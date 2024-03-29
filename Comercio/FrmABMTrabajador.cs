﻿using EntidadesComercio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Comercio
{
    public partial class FrmABMTrabajador : Form
    {
        private Usuario usuarioSeleccionado;

        #region Constructores

        public FrmABMTrabajador()
        {
            InitializeComponent();
        }
        #endregion

        #region Cargar info

        /// <summary>
        /// Carga info de trabajadores dependiendo el tipo de rango
        /// </summary>
        /// <param name="tipoSeleccionado"></param>
        private void CargarListaUsuariosPorTipo(EnumTipoUsuario tipoSeleccionado)
        {
            dgv_ListadoUsuarios.Columns[6].Visible = true;
            switch (tipoSeleccionado)
            {
                case EnumTipoUsuario.Administrador:
                    dgv_ListadoUsuarios.Rows.Clear();
                    dgv_ListadoUsuarios.Columns[6].Visible = false;
                    foreach (KeyValuePair<string, Usuario> item in CoreDelSistema.ListaUsuarios)
                    {
                        if (item.Value.TipoUsuario == EnumTipoUsuario.Administrador)
                        {
                            Administrador admin = (Administrador)item.Value;
                            dgv_ListadoUsuarios.Rows.Add($"{admin.Legajo}",
                                                $"{admin.Dni}",
                                                $"{admin.Nombre}",
                                                $"{admin.Apellido}",
                                                $"{admin.UsuarioPropiedad}",
                                                $"{admin.Sueldo}");
                        }
                    }
                break;
                case EnumTipoUsuario.Empleado:
                    dgv_ListadoUsuarios.Rows.Clear();
                    foreach (KeyValuePair<string, Usuario> item in CoreDelSistema.ListaUsuarios)
                    {
                        if (item.Value.TipoUsuario == EnumTipoUsuario.Empleado)
                        {
                            Empleado emp = (Empleado)item.Value;
                            dgv_ListadoUsuarios.Rows.Add($"{emp.Legajo}",
                                                    $"{emp.Dni}",
                                                    $"{emp.Nombre}",
                                                    $"{emp.Apellido}",
                                                    $"{emp.UsuarioPropiedad}",
                                                    $"{emp.Sueldo}",
                                                    $"{emp.CantidadVentas}");
                        }
                    }
                break;
                default:
                    dgv_ListadoUsuarios.Rows.Clear();
                break;
            }
        }

        /// <summary>
        /// Carga tipos de trabajador, admin o empleado al combobox
        /// </summary>
        private void CargarTiposDeUsuarios()
        {
            cmb_TipoUsuarios.Items.Add(EnumTipoUsuario.Administrador);
            cmb_TipoUsuarios.Items.Add(EnumTipoUsuario.Empleado);
        }

        #endregion

        #region Eventos
        /// <summary>
        /// Evento load, redimensiona la pantalla sin menus inferiores
        /// carga tipos de usuarios
        /// desabilita los botones elminar y modificar hasta seleccionar un
        /// usuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmBaseUsuario_Load(object sender, EventArgs e)
        {
            Size = new Size(865, 275);
            CargarTiposDeUsuarios();
            btn_EliminarUsuario.Enabled = false;
            btn_ModificarUsuario.Enabled = false;
        }
        #endregion

        /// <summary>
        /// Obtiene el dni de un Usuario desde la seleccion de la fila en el Dat Grid
        /// </summary>
        /// <returns>Dni encontrado si ok, sino vacio </returns>
        private string GetDniUsuarioPorDataGrid()
        {
            if(CoreDelSistema.ListaUsuarios.ContainsKey(this.dgv_ListadoUsuarios.CurrentRow.Cells[1].Value.ToString()))
            {
                return this.dgv_ListadoUsuarios.CurrentRow.Cells[1].Value.ToString(); 
            }
            else
            {
                btn_ModificarUsuario.Enabled = false;
                btn_EliminarUsuario.Enabled = false;
                lbl_DatoCapturado.BackColor = Color.Red;
                lbl_DatoCapturado.Text = "Error al querer obtener datos de Usuario seleccionado";
            }
            return string.Empty;
        }

        /// <summary>
        /// Elimina un usuario, trabajador
        /// Antes pregunta si deseea confirmar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_EliminarUsuario_Click(object sender, EventArgs e)
        {
            if (this.usuarioSeleccionado != null &&
                CoreDelSistema.UsuarioLogeado.Dni != this.usuarioSeleccionado.Dni)
            {
                if (MessageBox.Show($"{this.usuarioSeleccionado.MostrarDatosCompletos()}Desea confirmar la baja?", "Confirmacion, baja Usuario", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes &&
                    CoreDelSistema.ListaUsuarios.Remove(this.usuarioSeleccionado.Dni))
                {
                    lbl_DatoCapturado.BackColor = Color.Lime;
                    lbl_DatoCapturado.Text = "USUARIO ELIMINADO";

                    btn_EliminarUsuario.Enabled = false;
                    btn_ModificarUsuario.Enabled = false;
                    CargarListaUsuariosPorTipo(this.usuarioSeleccionado.TipoUsuario); // actualizo lista
                }
            }
            else
                MessageBox.Show("El usuario que intentas eliminar es el mismo con el que se inicio sesión",  "Informativo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /// <summary>
        /// Depenediendo del tipo de usuario se carga la lista
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb_TipoUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarListaUsuariosPorTipo((EnumTipoUsuario)cmb_TipoUsuarios.SelectedItem);
        }

        /// <summary>
        /// Boton que abre el menu de modificacion de trabajador
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ModificarUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.usuarioSeleccionado != null)
                {
                    Size = new Size(850, 435);
                    CompletarCamposAModificar(this.usuarioSeleccionado);
                }
            }
            catch
            {
                lbl_DatoCapturado.BackColor = Color.Red;
                lbl_DatoCapturado.Text = "ERROR al querer completar los campos del modificador de usuario";
            }
        }

        /// <summary>
        /// Completa los campos con los datos de usuario
        /// si eligio la opcion modificar
        /// </summary>
        /// <param name="usuarioAModificar"></param>
        private void CompletarCamposAModificar(Usuario usuarioAModificar)
        {
            if(usuarioAModificar != null)
            {
                txt_LegajoIngresado.Text = usuarioAModificar.Legajo.ToString();
                txt_LegajoIngresado.Enabled = false;
                txt_DniIngresado.Text = usuarioAModificar.Dni;
                txt_DniIngresado.Enabled = false;
                txt_NombreIngresado.Text = usuarioAModificar.Nombre;
                txt_ApellidoIngresado.Text = usuarioAModificar.Apellido;
                txt_UsuarioIngresado.Text = usuarioAModificar.UsuarioPropiedad;
                txt_ContraseñaIngresada.Text = usuarioAModificar.Contraseña;
                txt_UrlFotoPerfilIngresada.Text = usuarioAModificar.UrlImagen;

                if (usuarioSeleccionado.TipoUsuario is EnumTipoUsuario.Administrador)
                {
                    Administrador admin = (Administrador)usuarioAModificar;
                    txt_SueldoIngresado.Text = admin.Sueldo.ToString();
                }
                else if (usuarioSeleccionado.TipoUsuario is EnumTipoUsuario.Empleado)
                {
                    Empleado emp = (Empleado)usuarioAModificar;
                    txt_SueldoIngresado.Text = emp.Sueldo.ToString();
                }
            }
        }

        /// <summary>
        /// Confirma accion, ya sea ALTA - Modificacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Confirmar_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.usuarioSeleccionado != null && SonValidosCamposModificarUsuario())
                {
                    this.usuarioSeleccionado.Nombre = txt_NombreIngresado.Text;
                    this.usuarioSeleccionado.Apellido = txt_ApellidoIngresado.Text;
                    this.usuarioSeleccionado.UsuarioPropiedad = txt_UsuarioIngresado.Text;
                    this.usuarioSeleccionado.Contraseña = txt_ContraseñaIngresada.Text;
                    this.usuarioSeleccionado.UrlImagen = txt_UrlFotoPerfilIngresada.Text;

                    if (usuarioSeleccionado.TipoUsuario is EnumTipoUsuario.Administrador)
                    {
                        Administrador admin = (Administrador)this.usuarioSeleccionado;
                        admin.Sueldo = float.Parse(txt_SueldoIngresado.Text);
                    }
                    else if (usuarioSeleccionado.TipoUsuario is EnumTipoUsuario.Empleado)
                    {
                        Empleado emp = (Empleado)this.usuarioSeleccionado;
                        emp.Sueldo = float.Parse(txt_SueldoIngresado.Text);
                    }

                    lbl_DatoCapturado.BackColor = Color.Lime;
                    lbl_DatoCapturado.Text = "USUARIO MODIFICADO";
                    CargarListaUsuariosPorTipo(this.usuarioSeleccionado.TipoUsuario);
                    Size = new Size(865, 275);
                }
            }
            catch
            {
                lbl_DatoCapturado.BackColor = Color.Red;
                lbl_DatoCapturado.Text = "ERROR AL CONFIRMAR MODIFICACION";
            }
        }

        /// <summary>
        /// Verifica que los campos modificador de usuario no se encuentren vacios,
        /// y verifica que el sueld sea un numero parsable
        /// </summary>
        /// <returns>Retorna true si los campos son validos, sino false</returns>
        private bool SonValidosCamposModificarUsuario()
        {
            float sueldoValido;
            if(!String.IsNullOrEmpty(txt_NombreIngresado.Text) &&
               !String.IsNullOrEmpty(txt_ApellidoIngresado.Text) &&
               !String.IsNullOrEmpty(txt_UsuarioIngresado.Text) &&
               !String.IsNullOrEmpty(txt_ContraseñaIngresada.Text) &&
                float.TryParse(txt_SueldoIngresado.Text, out sueldoValido) &&
                Usuario.EsValidoDni(txt_DniIngresado.Text) &&
                Usuario.EsValidoNombreUsuario(txt_UsuarioIngresado.Text))
            {
                lbl_Error.Visible = false;
                btn_EliminarUsuario.Enabled = true;
                return true;
            }
            else
                lbl_Error.Visible = true;
            return false;
        }
        /// <summary>
        /// Si el legajo es valido habilita el buscador, sino sigue desabilitado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_LegajoABuscar_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int legajoValido;
                if (btn_BuscarLegajo.Text != string.Empty &&
                   int.TryParse(txt_LegajoABuscar.Text, out legajoValido) &&
                   legajoValido > 0 &&
                   legajoValido <= Usuario.UltimoLegajo)
                { 
                    btn_BuscarLegajo.Enabled = true;
                }
            }
            catch
            {
                lbl_DatoCapturado.BackColor = Color.Red;
                lbl_DatoCapturado.Text = "ERROR AL BUSCAR LEGAJO";
            }
        }

        /// <summary>
        /// Cierra pantalla abm usuario trabajador
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pb_CerrarAplicacion_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Boton que busca legajo y lo imprime por la lista
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_BuscarLegajo_Click(object sender, EventArgs e)
        {
            int legajoValido;
            if (txt_LegajoABuscar.Text != string.Empty &&
                int.TryParse(txt_LegajoABuscar.Text, out legajoValido) &&
                legajoValido > 0 &&
                legajoValido <= Usuario.UltimoLegajo)
            {
                this.usuarioSeleccionado = Usuario.BuscarUsuario(legajoValido);
                dgv_ListadoUsuarios.Rows.Clear();
                if(this.usuarioSeleccionado.TipoUsuario == EnumTipoUsuario.Administrador)
                {
                    dgv_ListadoUsuarios.Columns[6].Visible = false;
                    Administrador admin = (Administrador)this.usuarioSeleccionado;
                    dgv_ListadoUsuarios.Rows.Add($"{admin.Legajo}",
                                                 $"{admin.Dni}",
                                                 $"{admin.Nombre}",
                                                 $"{admin.Apellido}",
                                                 $"{admin.UsuarioPropiedad}",
                                                 $"{admin.Sueldo}");
                }
                else if (this.usuarioSeleccionado.TipoUsuario == EnumTipoUsuario.Empleado)
                {
                    dgv_ListadoUsuarios.Columns[6].Visible = true;
                    Empleado emp = (Empleado)this.usuarioSeleccionado;
                    dgv_ListadoUsuarios.Rows.Add($"{emp.Legajo}",
                                                 $"{emp.Dni}",
                                                 $"{emp.Nombre}",
                                                 $"{emp.Apellido}",
                                                 $"{emp.UsuarioPropiedad}",
                                                 $"{emp.Sueldo}",
                                                 $"{emp.CantidadVentas}");
                }
            }
            else
            {
                btn_BuscarLegajo.Enabled = false;
                lbl_DatoCapturado.BackColor = Color.Red;
                lbl_DatoCapturado.Text = "ERROR, el legajo ingresado no existe";
            }
        }

        /// <summary>
        /// Selecciona un usuario del data grid, obtiene el dato dni en este caso
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_ListadoUsuarios_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (GetDniUsuarioPorDataGrid() != string.Empty)
                {
                    Size = new Size(865, 275);
                    this.usuarioSeleccionado = CoreDelSistema.ListaUsuarios[GetDniUsuarioPorDataGrid()]; // inicializo usuario seleccionado del Data Grid
                    pb_ImagenUsuario.ImageLocation = this.usuarioSeleccionado.UrlImagen;
                    lbl_DatoCapturado.BackColor = Color.Beige;
                    lbl_DatoCapturado.Text = $"{this.usuarioSeleccionado.TipoUsuario} seleccionado: {this.usuarioSeleccionado.Nombre} {this.usuarioSeleccionado.Apellido}";

                    // Habilito botones para poder modificar o eliminar usuario seleccionado
                    btn_ModificarUsuario.Enabled = true;
                    btn_EliminarUsuario.Enabled = true;
                    Console.Beep();
                }
            }
            catch
            {
                lbl_DatoCapturado.BackColor = Color.Red;
                lbl_DatoCapturado.Text = "ERROR al querer capturar datos del usuario selecionado";
            }
        }
        /// <summary>
        /// Abre el forms de alta de usuario trabajador
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AgregarUsuario_Click(object sender, EventArgs e)
        {
            FrmAltaTrabajador formAltaTrabajador = new FrmAltaTrabajador();
            formAltaTrabajador.ShowDialog();
        }
    }
}
