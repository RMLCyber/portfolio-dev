package com.agenda;

import com.agenda.controller.AddContactController;
import com.agenda.db.ContactDAO;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;

public class Main extends Application {
    @Override
    public void start(Stage primaryStage) {
        try {
            // 🔧 Création du chargeur FXML
            FXMLLoader loader = new FXMLLoader(getClass().getResource("/views/AddContactView.fxml"));

            // 🔄 Chargement du fichier FXML
            Parent root = loader.load();

            // ✅ Récupération du contrôleur
            AddContactController controller = loader.getController();

            // 🔌 Injection du DAOs
            ContactDAO dao = new ContactDAO();
            controller.setContactDAO(dao);

            // 🎬 Configuration et affichage de la scène
            primaryStage.setTitle("Agenda Électronique");
            primaryStage.setScene(new Scene(root, 400, 300));
            primaryStage.show();

        } catch (Exception e) {
            System.err.println("⚠️ Une erreur est survenue au lancement de l'application :");
            e.printStackTrace();
        }
    }

    public static void main(String[] args) {
        launch(args);
    }
}
