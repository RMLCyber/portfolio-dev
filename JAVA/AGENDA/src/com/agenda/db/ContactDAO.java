package com.agenda.db;

import com.agenda.model.Contact;

import java.sql.*;
import java.util.ArrayList;

public class ContactDAO {

    private static final String URL = "jdbc:oracle:thin:@localhost:1521:XE";
    private static final String USER = "SYSTEM";
    private static final String PASSWORD = "43026172LNRm@";

    private Connection getConnection() throws SQLException {
        return DriverManager.getConnection(URL, USER, PASSWORD);
    }

    public void ajouterContact(Contact contact) throws Exception {
        String sql = "INSERT INTO contacts (nom, email, telephone) VALUES (?, ?, ?)";
        try (Connection conn = getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setString(1, contact.getNom());
            stmt.setString(2, contact.getEmail());
            stmt.setString(3, contact.getTelephone());

            stmt.executeUpdate();
        } catch (SQLException e) {
            throw new Exception("Erreur lors de l'ajout du contact : " + e.getMessage(), e);
        }
    }

    public ArrayList<Contact> recupererTousLesContacts() throws Exception {
        ArrayList<Contact> contacts = new ArrayList<>();
        String sql = "SELECT nom, email, telephone FROM contacts";

        try (Connection conn = getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql);
             ResultSet rs = stmt.executeQuery()) {

            while (rs.next()) {
                String nom = rs.getString("nom");
                String email = rs.getString("email");
                String telephone = rs.getString("telephone");

                contacts.add(new Contact(nom, email, telephone));
            }
        } catch (SQLException e) {
            throw new Exception("Erreur lors de la récupération des contacts : " + e.getMessage(), e);
        }

        return contacts;
    }

    public void supprimerContact(Contact contact) throws Exception {
        String sql = "DELETE FROM contacts WHERE nom = ? AND email = ? AND telephone = ?";
        try (Connection conn = getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setString(1, contact.getNom());
            stmt.setString(2, contact.getEmail());
            stmt.setString(3, contact.getTelephone());

            stmt.executeUpdate();
        } catch (SQLException e) {
            throw new Exception("Erreur lors de la suppression du contact : " + e.getMessage(), e);
        }
    }
}
