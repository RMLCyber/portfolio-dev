Imports System.Data.OleDb
Imports ADOX
Imports System.IO

Public Class Form1

    Dim dbPath As String = System.IO.Path.Combine(Application.StartupPath, "futur.accdb")
    Dim con As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & dbPath)
    Dim cmd As OleDbCommand
    Dim dt As DataTable
    Dim da As OleDbDataAdapter

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not IO.File.Exists(dbPath) Then
            Try
                Dim cat As New Catalog()
                cat.Create("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & dbPath)
                cat = Nothing
                MessageBox.Show("Base de données créée automatiquement !")
                CreerTables()
                AjouterDonneesSiVide()
            Catch ex As Exception
                MessageBox.Show("Erreur lors de la création de la base de données : " & ex.Message)
            End Try
        Else
            AjouterDonneesSiVide()
        End If

        ChargerComboBox("Jour", "Nom_Jour", "ID_Jour", ComboBox1)
        ChargerHeure()
        ChargerComboBox("Professeur", "Nom_Professeur", "ID_Professeur", ComboBox3)
        ChargerComboBox("Salle", "Nom_Salle", "ID_Salle", ComboBox4)
        ChargerDataGrid()

        Timer1.Interval = 10000
        Timer1.Start()

        ' Empêcher la saisie manuelle dans les ComboBox
        ComboBox1.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBox2.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBox3.DropDownStyle = ComboBoxStyle.DropDown
        ComboBox4.DropDownStyle = ComboBoxStyle.DropDown

        ' Lier les événements pour arrêter/redémarrer le timer
        AddHandler TextBox1.Enter, AddressOf ArreterTimerEdition
        AddHandler TextBox1.Leave, AddressOf RedemarrerTimerEdition
        AddHandler ComboBox1.Enter, AddressOf ArreterTimerEdition
        AddHandler ComboBox1.Leave, AddressOf RedemarrerTimerEdition
        AddHandler ComboBox2.Enter, AddressOf ArreterTimerEdition
        AddHandler ComboBox2.Leave, AddressOf RedemarrerTimerEdition
        AddHandler ComboBox3.Enter, AddressOf ArreterTimerEdition
        AddHandler ComboBox3.Leave, AddressOf RedemarrerTimerEdition
        AddHandler ComboBox4.Enter, AddressOf ArreterTimerEdition
        AddHandler ComboBox4.Leave, AddressOf RedemarrerTimerEdition
        AddHandler DataGridView1.Enter, AddressOf ArreterTimerEdition
        AddHandler DataGridView1.Leave, AddressOf RedemarrerTimerEdition
    End Sub

    Private Sub ArreterTimerEdition(sender As Object, e As EventArgs)
        Timer1.Stop()
    End Sub

    Private Sub RedemarrerTimerEdition(sender As Object, e As EventArgs)
        If Not (TextBox1.Focused Or ComboBox1.Focused Or ComboBox2.Focused Or ComboBox3.Focused Or ComboBox4.Focused Or DataGridView1.Focused) Then
            Timer1.Start()
        End If
    End Sub

    Private Sub CreerTables()
        Try
            con.Open()
            Dim sqls As String() = {
                "CREATE TABLE Jour (ID_Jour AUTOINCREMENT PRIMARY KEY, Nom_Jour TEXT(50))",
                "CREATE TABLE Heure (ID_Heure AUTOINCREMENT PRIMARY KEY, Heure_Debut TEXT(20), Heure_Fin TEXT(20))",
                "CREATE TABLE Professeur (ID_Professeur AUTOINCREMENT PRIMARY KEY, Nom_Professeur TEXT(50))",
                "CREATE TABLE Salle (ID_Salle AUTOINCREMENT PRIMARY KEY, Nom_Salle TEXT(50))",
                "CREATE TABLE Cours (" &
                    "ID_Cours AUTOINCREMENT PRIMARY KEY, " &
                    "Nom_Cours TEXT(100), " &
                    "ID_Jour INT, " &
                    "ID_Heure INT, " &
                    "ID_Professeur INT, " &
                    "ID_Salle INT" &
                ")"
            }

            For Each sql In sqls
                cmd = New OleDbCommand(sql, con)
                cmd.ExecuteNonQuery()
            Next

            MessageBox.Show("Tables créées avec succès !")
        Catch ex As Exception
            MessageBox.Show("Erreur lors de la création des tables : " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Private Sub AjouterDonneesSiVide()
        Try
            con.Open()

            If EstTableVide("Jour") Then
                Dim jours As String() = {"Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi"}
                For Each jour In jours
                    cmd = New OleDbCommand("INSERT INTO Jour (Nom_Jour) VALUES (?)", con)
                    cmd.Parameters.AddWithValue("?", jour)
                    cmd.ExecuteNonQuery()
                Next
            End If

            If EstTableVide("Heure") Then
                Dim heures As (String, String)() = {
                    ("08:00", "09:00"),
                    ("09:00", "10:00"),
                    ("10:00", "11:00"),
                    ("11:00", "12:00"),
                    ("13:00", "14:00"),
                    ("14:00", "15:00"),
                    ("15:00", "16:00"),
                    ("16:00", "17:00")
                }
                For Each h In heures
                    cmd = New OleDbCommand("INSERT INTO Heure (Heure_Debut, Heure_Fin) VALUES (?, ?)", con)
                    cmd.Parameters.AddWithValue("?", h.Item1)
                    cmd.Parameters.AddWithValue("?", h.Item2)
                    cmd.ExecuteNonQuery()
                Next
            End If

            If EstTableVide("Professeur") Then
                Dim profs As String() = {"Mr. Kouadio", "Mme. Koné"}
                For Each prof In profs
                    cmd = New OleDbCommand("INSERT INTO Professeur (Nom_Professeur) VALUES (?)", con)
                    cmd.Parameters.AddWithValue("?", prof)
                    cmd.ExecuteNonQuery()
                Next
            End If

            If EstTableVide("Salle") Then
                Dim salles As String() = {"Salle A", "Salle B"}
                For Each salle In salles
                    cmd = New OleDbCommand("INSERT INTO Salle (Nom_Salle) VALUES (?)", con)
                    cmd.Parameters.AddWithValue("?", salle)
                    cmd.ExecuteNonQuery()
                Next
            End If

        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'ajout des données par défaut : " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    Private Function EstTableVide(nomTable As String) As Boolean
        Dim count As Integer = 0
        Try
            Dim cmdCount As New OleDbCommand("SELECT COUNT(*) FROM " & nomTable, con)
            count = Convert.ToInt32(cmdCount.ExecuteScalar())
        Catch ex As Exception
            MessageBox.Show("Erreur lors de la vérification de la table " & nomTable & " : " & ex.Message)
        End Try
        Return (count = 0)
    End Function

    Private Sub ChargerComboBox(nomTable As String, champAffichage As String, champValeur As String, combo As ComboBox)
        Try
            Dim da As New OleDbDataAdapter("SELECT * FROM " & nomTable, con)
            Dim dt As New DataTable()
            da.Fill(dt)
            combo.DataSource = dt
            combo.DisplayMember = champAffichage
            combo.ValueMember = champValeur
        Catch ex As Exception
            MessageBox.Show("Erreur lors du chargement de [" & nomTable & "] : " & ex.Message)
        End Try
    End Sub

    Private Sub ChargerHeure()
        Try
            Dim query As String = "SELECT ID_Heure, (Heure_Debut & ' - ' & Heure_Fin) AS Plage_Horaire FROM Heure"
            Dim da As New OleDbDataAdapter(query, con)
            Dim dt As New DataTable()
            da.Fill(dt)
            ComboBox2.DataSource = dt
            ComboBox2.DisplayMember = "Plage_Horaire"
            ComboBox2.ValueMember = "ID_Heure"
        Catch ex As Exception
            MessageBox.Show("Erreur lors du chargement des heures : " & ex.Message)
        End Try
    End Sub

    Private Sub ChargerDataGrid()
        Try
            If con.State = ConnectionState.Open Then con.Close()
            con.Open()

            Dim query As String = "
                SELECT 
                    Cours.ID_Cours, 
                    Nom_Cours, 
                    Nom_Jour, 
                    Heure.ID_Heure,
                    (Heure_Debut & ' - ' & Heure_Fin) AS Plage_Horaire,
                    Nom_Professeur, 
                    Nom_Salle,
                    Cours.ID_Jour,
                    Cours.ID_Professeur,
                    Cours.ID_Salle
                FROM (((Cours 
                    INNER JOIN Jour ON Cours.ID_Jour = Jour.ID_Jour) 
                    INNER JOIN Heure ON Cours.ID_Heure = Heure.ID_Heure) 
                    INNER JOIN Professeur ON Cours.ID_Professeur = Professeur.ID_Professeur) 
                    INNER JOIN Salle ON Cours.ID_Salle = Salle.ID_Salle
                ORDER BY Jour.ID_Jour, Heure.Heure_Debut"

            da = New OleDbDataAdapter(query, con)
            dt = New DataTable()
            da.Fill(dt)
            DataGridView1.DataSource = dt
        Catch ex As Exception
            MessageBox.Show("Erreur lors du chargement des cours : " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    ' Ajout d'un cours avec gestion de la saisie manuelle Professeur/Salle
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            con.Open()

            ' --- Gérer Professeur ---
            Dim idProf As Integer
            Dim nomProf As String = ComboBox3.Text.Trim()
            Dim cmdProf As New OleDbCommand("SELECT ID_Professeur FROM Professeur WHERE Nom_Professeur = ?", con)
            cmdProf.Parameters.AddWithValue("?", nomProf)
            Dim resultProf = cmdProf.ExecuteScalar()
            If resultProf IsNot Nothing Then
                idProf = Convert.ToInt32(resultProf)
            Else
                Dim cmdInsertProf As New OleDbCommand("INSERT INTO Professeur (Nom_Professeur) VALUES (?)", con)
                cmdInsertProf.Parameters.AddWithValue("?", nomProf)
                cmdInsertProf.ExecuteNonQuery()
                cmdProf = New OleDbCommand("SELECT @@IDENTITY", con)
                idProf = Convert.ToInt32(cmdProf.ExecuteScalar())
                ChargerComboBox("Professeur", "Nom_Professeur", "ID_Professeur", ComboBox3)
            End If

            ' --- Gérer Salle ---
            Dim idSalle As Integer
            Dim nomSalle As String = ComboBox4.Text.Trim()
            Dim cmdSalle As New OleDbCommand("SELECT ID_Salle FROM Salle WHERE Nom_Salle = ?", con)
            cmdSalle.Parameters.AddWithValue("?", nomSalle)
            Dim resultSalle = cmdSalle.ExecuteScalar()
            If resultSalle IsNot Nothing Then
                idSalle = Convert.ToInt32(resultSalle)
            Else
                Dim cmdInsertSalle As New OleDbCommand("INSERT INTO Salle (Nom_Salle) VALUES (?)", con)
                cmdInsertSalle.Parameters.AddWithValue("?", nomSalle)
                cmdInsertSalle.ExecuteNonQuery()
                cmdSalle = New OleDbCommand("SELECT @@IDENTITY", con)
                idSalle = Convert.ToInt32(cmdSalle.ExecuteScalar())
                ChargerComboBox("Salle", "Nom_Salle", "ID_Salle", ComboBox4)
            End If

            ' --- Insertion du cours ---
            cmd = New OleDbCommand("INSERT INTO Cours (Nom_Cours, ID_Jour, ID_Heure, ID_Professeur, ID_Salle) VALUES (?, ?, ?, ?, ?)", con)
            cmd.Parameters.AddWithValue("?", TextBox1.Text)
            cmd.Parameters.AddWithValue("?", ComboBox1.SelectedValue)
            cmd.Parameters.AddWithValue("?", ComboBox2.SelectedValue)
            cmd.Parameters.AddWithValue("?", idProf)
            cmd.Parameters.AddWithValue("?", idSalle)
            cmd.ExecuteNonQuery()
            MessageBox.Show("Cours enregistré avec succès !")
            ChargerDataGrid()
        Catch ex As Exception
            MessageBox.Show("Erreur : " & ex.Message)
        Finally
            con.Close()
        End Try
    End Sub

    ' Modification d'un cours avec gestion de la saisie manuelle Professeur/Salle
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If DataGridView1.SelectedRows.Count > 0 Then
            Dim idCours As Integer = Convert.ToInt32(DataGridView1.SelectedRows(0).Cells("ID_Cours").Value)
            Try
                con.Open()

                ' --- Gérer Professeur ---
                Dim idProf As Integer
                Dim nomProf As String = ComboBox3.Text.Trim()
                Dim cmdProf As New OleDbCommand("SELECT ID_Professeur FROM Professeur WHERE Nom_Professeur = ?", con)
                cmdProf.Parameters.AddWithValue("?", nomProf)
                Dim resultProf = cmdProf.ExecuteScalar()
                If resultProf IsNot Nothing Then
                    idProf = Convert.ToInt32(resultProf)
                Else
                    Dim cmdInsertProf As New OleDbCommand("INSERT INTO Professeur (Nom_Professeur) VALUES (?)", con)
                    cmdInsertProf.Parameters.AddWithValue("?", nomProf)
                    cmdInsertProf.ExecuteNonQuery()
                    cmdProf = New OleDbCommand("SELECT @@IDENTITY", con)
                    idProf = Convert.ToInt32(cmdProf.ExecuteScalar())
                    ChargerComboBox("Professeur", "Nom_Professeur", "ID_Professeur", ComboBox3)
                End If

                ' --- Gérer Salle ---
                Dim idSalle As Integer
                Dim nomSalle As String = ComboBox4.Text.Trim()
                Dim cmdSalle As New OleDbCommand("SELECT ID_Salle FROM Salle WHERE Nom_Salle = ?", con)
                cmdSalle.Parameters.AddWithValue("?", nomSalle)
                Dim resultSalle = cmdSalle.ExecuteScalar()
                If resultSalle IsNot Nothing Then
                    idSalle = Convert.ToInt32(resultSalle)
                Else
                    Dim cmdInsertSalle As New OleDbCommand("INSERT INTO Salle (Nom_Salle) VALUES (?)", con)
                    cmdInsertSalle.Parameters.AddWithValue("?", nomSalle)
                    cmdInsertSalle.ExecuteNonQuery()
                    cmdSalle = New OleDbCommand("SELECT @@IDENTITY", con)
                    idSalle = Convert.ToInt32(cmdSalle.ExecuteScalar())
                    ChargerComboBox("Salle", "Nom_Salle", "ID_Salle", ComboBox4)
                End If

                cmd = New OleDbCommand("UPDATE Cours SET Nom_Cours=?, ID_Jour=?, ID_Heure=?, ID_Professeur=?, ID_Salle=? WHERE ID_Cours=?", con)
                cmd.Parameters.AddWithValue("?", TextBox1.Text)
                cmd.Parameters.AddWithValue("?", ComboBox1.SelectedValue)
                cmd.Parameters.AddWithValue("?", ComboBox2.SelectedValue)
                cmd.Parameters.AddWithValue("?", idProf)
                cmd.Parameters.AddWithValue("?", idSalle)
                cmd.Parameters.AddWithValue("?", idCours)
                cmd.ExecuteNonQuery()
                MessageBox.Show("Cours modifié avec succès !")
                ChargerDataGrid()
            Catch ex As Exception
                MessageBox.Show("Erreur : " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Veuillez sélectionner une ligne à modifier.")
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If DataGridView1.SelectedRows.Count > 0 Then
            Dim idCours As Integer = Convert.ToInt32(DataGridView1.SelectedRows(0).Cells("ID_Cours").Value)
            Try
                con.Open()
                cmd = New OleDbCommand("DELETE FROM Cours WHERE ID_Cours=?", con)
                cmd.Parameters.AddWithValue("?", idCours)
                cmd.ExecuteNonQuery()
                MessageBox.Show("Cours supprimé avec succès !")
                ChargerDataGrid()
            Catch ex As Exception
                MessageBox.Show("Erreur : " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Veuillez sélectionner une ligne à supprimer.")
        End If
    End Sub

    ' EXPORT CSV corrigé (parenthèses sur les jointures multiples)
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Try
            Dim jours As New List(Of (ID As Integer, Nom As String))
            Dim plages As New List(Of (ID As Integer, Plage As String))

            ' Charger les jours avec leur ID
            con.Open()
            Dim cmdJours As New OleDbCommand("SELECT ID_Jour, Nom_Jour FROM Jour ORDER BY ID_Jour", con)
            Dim rdrJours = cmdJours.ExecuteReader()
            While rdrJours.Read()
                jours.Add((CInt(rdrJours("ID_Jour")), rdrJours("Nom_Jour").ToString()))
            End While
            rdrJours.Close()

            ' Charger les plages horaires avec leur ID
            Dim cmdPlages As New OleDbCommand("SELECT ID_Heure, Heure_Debut & ' - ' & Heure_Fin AS Plage FROM Heure ORDER BY ID_Heure", con)
            Dim rdrPlages = cmdPlages.ExecuteReader()
            While rdrPlages.Read()
                plages.Add((CInt(rdrPlages("ID_Heure")), rdrPlages("Plage").ToString()))
            End While
            rdrPlages.Close()
            con.Close()

            Dim sfd As New SaveFileDialog()
            sfd.Filter = "Fichiers CSV (*.csv)|*.csv"
            sfd.FileName = "EmploiDuTemps.csv"

            If sfd.ShowDialog() = DialogResult.OK Then
                Dim filePath = sfd.FileName

                Using writer As New IO.StreamWriter(filePath, False, System.Text.Encoding.UTF8)
                    writer.WriteLine("Heure / Jour;" & String.Join(";", jours.Select(Function(j) j.Nom)))

                    con.Open()
                    For Each plage In plages
                        Dim ligne As New List(Of String)
                        ligne.Add(plage.Plage)

                        For Each jour In jours
                            Dim coursTexte As String = ""
                            Dim query As String = "SELECT Nom_Cours & ' (' & Nom_Professeur & ', ' & Nom_Salle & ')' AS InfoCours " &
                                "FROM ((Cours " &
                                "INNER JOIN Professeur ON Cours.ID_Professeur = Professeur.ID_Professeur) " &
                                "INNER JOIN Salle ON Cours.ID_Salle = Salle.ID_Salle) " &
                                "WHERE Cours.ID_Jour = ? AND Cours.ID_Heure = ?"
                            Try
                                Dim cmdCours As New OleDbCommand(query, con)
                                cmdCours.Parameters.AddWithValue("?", jour.ID)
                                cmdCours.Parameters.AddWithValue("?", plage.ID)
                                Dim readerCours = cmdCours.ExecuteReader()
                                If readerCours.Read() Then
                                    coursTexte = readerCours("InfoCours").ToString()
                                End If
                                readerCours.Close()
                            Catch ex As Exception
                                coursTexte = "Erreur: " & ex.Message & vbCrLf & "REQUETE: " & query
                            End Try
                            ligne.Add(coursTexte)
                        Next
                        writer.WriteLine(String.Join(";", ligne))
                    Next
                    con.Close()
                End Using

                MessageBox.Show("Export CSV terminé avec succès !")
            End If

        Catch ex As Exception
            MessageBox.Show("Erreur lors de l'export CSV : " & ex.Message)
        End Try
    End Sub

    Private Sub DataGridView1_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridView1.SelectionChanged
        If DataGridView1.SelectedRows.Count > 0 Then
            Timer1.Stop()
            Dim row = DataGridView1.SelectedRows(0)

            If Not IsDBNull(row.Cells("Nom_Cours").Value) Then
                TextBox1.Text = row.Cells("Nom_Cours").Value.ToString()
            Else
                TextBox1.Text = ""
            End If

            If Not IsDBNull(row.Cells("ID_Jour").Value) Then
                ComboBox1.SelectedValue = Convert.ToInt32(row.Cells("ID_Jour").Value)
            Else
                ComboBox1.SelectedIndex = -1
            End If

            If Not IsDBNull(row.Cells("ID_Heure").Value) Then
                ComboBox2.SelectedValue = Convert.ToInt32(row.Cells("ID_Heure").Value)
            Else
                ComboBox2.SelectedIndex = -1
            End If

            If Not IsDBNull(row.Cells("ID_Professeur").Value) Then
                ComboBox3.SelectedValue = Convert.ToInt32(row.Cells("ID_Professeur").Value)
            Else
                ComboBox3.SelectedIndex = -1
            End If

            If Not IsDBNull(row.Cells("ID_Salle").Value) Then
                ComboBox4.SelectedValue = Convert.ToInt32(row.Cells("ID_Salle").Value)
            Else
                ComboBox4.SelectedIndex = -1
            End If
        Else
            Timer1.Start()
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ChargerDataGrid()
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged

    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged

    End Sub
End Class
