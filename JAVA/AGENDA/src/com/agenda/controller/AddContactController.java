package com.agenda.controller;

import com.agenda.db.ContactDAO;
import com.agenda.model.Contact;

import javafx.collections.FXCollections;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.beans.property.SimpleStringProperty;

import java.util.ArrayList;
import java.util.stream.Collectors;

public class AddContactController {

    @FXML
    private TextField nomField;

    @FXML
    private TextField emailField;

    @FXML
    private TextField telephoneField;

    @FXML
    private Label messageLabel;

    @FXML
    private TableView<Contact> contactTable;

    @FXML
    private TableColumn<Contact, String> nomColumn;

    @FXML
    private TableColumn<Contact, String> emailColumn;

    @FXML
    private TableColumn<Contact, String> telephoneColumn;

    private ContactDAO contactDAO;

    public void setContactDAO(ContactDAO contactDAO) {
        this.contactDAO = contactDAO;
        chargerContacts(); //  Charger les contacts une fois le DAO prêt
    }

    @FXML
    private void initialize() {
        nomColumn.setCellValueFactory(data -> new SimpleStringProperty(data.getValue().getNom()));
        emailColumn.setCellValueFactory(data -> new SimpleStringProperty(data.getValue().getEmail()));
        telephoneColumn.setCellValueFactory(data -> new SimpleStringProperty(data.getValue().getTelephone()));
        //  Ne pas appeler chargerContacts ici, car contactDAO n’est pas encore injecté
    }

    @FXML
    private void addContact() {
        String nom = nomField.getText();
        String email = emailField.getText();
        String telephone = telephoneField.getText();

        Contact contact = new Contact(nom, email, telephone);

        try {
            contactDAO.ajouterContact(contact);
            messageLabel.setText("Contact ajouté avec succès !");
            chargerContacts();
        } catch (Exception e) {
            messageLabel.setText("Erreur : " + e.getMessage());
            e.printStackTrace();
        }
    }

    @FXML
    private void annulerFormulaire() {
        nomField.clear();
        emailField.clear();
        telephoneField.clear();
        messageLabel.setText("Formulaire réinitialisé.");
    }

    @FXML
    private void rechercherContact() {
        String nomRecherche = nomField.getText().toLowerCase().trim();

        if (nomRecherche.isEmpty()) {
            messageLabel.setText("Champ de recherche vide.");
            return;
        }

        try {
            ArrayList<Contact> tousLesContacts = contactDAO.recupererTousLesContacts();
            ArrayList<Contact> resultats = tousLesContacts.stream()
                    .filter(c -> c.getNom().toLowerCase().contains(nomRecherche))
                    .collect(Collectors.toCollection(ArrayList::new));
            contactTable.setItems(FXCollections.observableArrayList(resultats));
            messageLabel.setText(resultats.isEmpty() ? "Aucun contact trouvé." : resultats.size() + " contact(s) trouvé(s).");
        } catch (Exception e) {
            messageLabel.setText("Erreur recherche : " + e.getMessage());
            e.printStackTrace();
        }
    }

    @FXML
    private void supprimerContact() {
        Contact selected = contactTable.getSelectionModel().getSelectedItem();

        if (selected != null) {
            try {
                contactDAO.supprimerContact(selected);
                chargerContacts();
                messageLabel.setText("Contact supprimé avec succès.");
            } catch (Exception e) {
                messageLabel.setText("Erreur suppression : " + e.getMessage());
                e.printStackTrace();
            }
        } else {
            messageLabel.setText("Sélectionne un contact à supprimer.");
        }
    }

    private void chargerContacts() {
        try {
            ArrayList<Contact> contacts = contactDAO.recupererTousLesContacts();
            contactTable.setItems(FXCollections.observableArrayList(contacts));

            // 🧪 Log de vérification des données récupérées
            System.out.println("🧪 Vérification du contenu Oracle :");
            for (Contact c : contacts) {
                System.out.println(c.getNom() + " | " + c.getEmail() + " | " + c.getTelephone());
            }
        } catch (Exception e) {
            messageLabel.setText("Erreur lors du chargement des contacts.");
            e.printStackTrace();
        }
    }
}
